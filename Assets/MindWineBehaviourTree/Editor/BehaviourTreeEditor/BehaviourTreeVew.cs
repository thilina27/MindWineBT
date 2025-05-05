// © Thilina

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BT.Runtime.Attributes;
using BT.Runtime.Nodes;
using BT.Runtime.Nodes.CompositeNodes;
using BT.Runtime.Nodes.DecoratorNodes;
using BT.Runtime.Nodes.LeafNodes;
using BT.Runtime.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    /// <summary>
    /// Extend graph view to show this in ui toolkit
    /// </summary>
    public class BehaviourTreeVew : GraphView
    {

        /// <summary>
        /// Mark it so it will show in UI builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BehaviourTreeVew, UxmlTraits>
        {
        }

        public Action<BehaviourTreeNode> OnNodeSelected;

        private BehaviourTree _behaviourTree;

        public BehaviourTreeVew()
        {
            // insert grid background 
            // comes from graph view
            Insert(0, new GridBackground());

            // add manipulators 
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentZoomer());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPathResolver.GetTreeEditorUSSPath());
            styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// populate the graph view
        /// </summary>
        /// <param name="selectedTree"></param>
        public void PopulateView(BehaviourTree selectedTree)
        {
            OnNodeSelected?.Invoke(null);

            _behaviourTree = selectedTree;

            // we do event unsub/sub tp prevent handle when cleaning
            graphViewChanged -= OnGraphViewChanged;
            // clear graph
            DeleteElements(graphElements);

            graphViewChanged += OnGraphViewChanged;

            if (!selectedTree)
            {
                // TODO : some thing to indicate that no tree is selected
                return;
            }

            // check for root 
            if (_behaviourTree.RootNode == null)
            {
                var root = _behaviourTree.CreateNode(typeof(RootNode));
                _behaviourTree.AddRoot(root);
            }

            // create all nodes
            foreach (var node in _behaviourTree.Nodes)
            {
                CreateNodeView(node);
            }

            // create all edges, this need all nodes created 
            foreach (var node in _behaviourTree.Nodes)
            {
                CreateEdges(node);
            }
        }

        /// <summary>
        /// When undo redo action performed in the graph editor
        /// </summary>
        public void UndoRedoPerformed()
        {
            PopulateView(_behaviourTree);
            AssetDatabase.SaveAssets();
        }

        // trigger when graph changed
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                HandleDeletion(graphViewChange);
            }

            // handle edge creation 
            if (graphViewChange.edgesToCreate != null)
            {
                HandleEdgeCreation(graphViewChange);
            }

            // handle move 
            if (graphViewChange.movedElements != null)
            {
                HandleMove();
            }

            return graphViewChange;
        }

        // when movement detected sort composite children to have execution order
        private void HandleMove()
        {
            foreach (var node in nodes)
            {
                if (node is BehaviourTreeNodeView nodeView)
                {
                    nodeView.SortChildren();
                }
            }
        }

        private void HandleDeletion(GraphViewChange graphViewChange)
        {
            // we go though all elements to removed 
            foreach (var graphElement in graphViewChange.elementsToRemove)
            {
                // deleting a node view
                if (graphElement is BehaviourTreeNodeView nodeView)
                {
                    _behaviourTree.RemoveNode(nodeView.BehaviourTreeNode);
                }

                // remove edge
                if (graphElement is Edge edge)
                {
                    if (edge.output.node is BehaviourTreeNodeView parentNode && edge.input.node is BehaviourTreeNodeView childNode)
                    {
                        _behaviourTree.RemoveChild(parentNode.BehaviourTreeNode, childNode.BehaviourTreeNode);
                    }
                }
            }
        }

        private void HandleEdgeCreation(GraphViewChange graphViewChange)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                if (edge.output.node is BehaviourTreeNodeView parentView && edge.input.node is BehaviourTreeNodeView childView)
                {
                    _behaviourTree.AddChild(parentView.BehaviourTreeNode, childView.BehaviourTreeNode);
                }
            }
        }

        // This needs to be overridden to make the ports match each other to make them linkable
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // return base.GetCompatiblePorts(startPort, nodeAdapter);
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && startPort.node != endPort.node).ToList();
        }

        /// <summary>
        /// On right click inside graph
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_behaviourTree == null)
            {
                return;
            }

            evt.menu.AppendSeparator();

            var types = TypeCache.GetTypesDerivedFrom<LeafNode>();
            foreach (var type in types)
            {
                CreateMenuItem(type, nameof(LeafNode), type.Name, evt);
            }

            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                CreateMenuItem(type, nameof(CompositeNode), type.Name, evt);
            }

            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                CreateMenuItem(type, nameof(DecoratorNode), type.Name, evt);
            }

            if (evt.target is GraphView or Node or Group or Edge)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Delete", _ => DeleteSelectionCallback(AskUser.DontAskUser),
                    _ => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }
        }

        // update all nodes
        public void UpdateTreeView()
        {
            foreach (var node in nodes)
            {
                if (node is BehaviourTreeNodeView nodeView)
                {
                    nodeView.UpdateNodeView();
                }
            }
        }

        private void CreateMenuItem(Type type, string defaultMenu, string defaultItemName, ContextualMenuPopulateEvent evt)
        {
            var attribute = type.GetCustomAttribute<CreateNodeMenu>();
            var menuName = defaultMenu;
            var itemName = defaultItemName;

            if (attribute != null)
            {
                menuName = attribute.MenuPath == "" ? menuName : attribute.MenuPath; // TODO : cut trail / if in it
                itemName = attribute.ItemName == "" ? itemName : attribute.ItemName;
            }

            evt.menu.AppendAction($"{menuName}/ {itemName}", a => CreateNode(type, a.eventInfo.mousePosition));
        }

        private void CreateNode(Type type, Vector2 eventInfoMousePosition)
        {
            var worldPos = ElementAt(0).LocalToWorld(eventInfoMousePosition);
            var localPos = ElementAt(1).WorldToLocal(worldPos);
            var node = _behaviourTree.CreateNode(type);
            node.Position = localPos; // TODO: fix mouse position 
            CreateNodeView(node);
        }

        private void CreateNodeView(BehaviourTreeNode treeNode)
        {
            var nodeView = new BehaviourTreeNodeView(treeNode)
            {
                OnNodeSelected = OnNodeSelected
            };
            AddElement(nodeView);
        }

        private void CreateEdges(BehaviourTreeNode node)
        {
            var parentView = FindNodeView(node);
            foreach (var child in _behaviourTree.GetChildren(node))
            {
                var childView = FindNodeView(child);
                var linkFrom = parentView.OutputPort.ConnectTo(childView.InputPort);
                var linkTo = childView.InputPort.ConnectTo(parentView.OutputPort);

                AddElement(linkFrom);
                AddElement(linkTo);
            }
        }

        private BehaviourTreeNodeView FindNodeView(BehaviourTreeNode treeNode)
        {
            return GetNodeByGuid(treeNode.Guid) as BehaviourTreeNodeView;
        }
    }
}
