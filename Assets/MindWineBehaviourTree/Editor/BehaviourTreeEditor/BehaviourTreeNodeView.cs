// © Thilina

using System;
using BT.Runtime.Nodes;
using BT.Runtime.Nodes.CompositeNodes;
using BT.Runtime.Nodes.DecoratorNodes;
using BT.Runtime.Nodes.LeafNodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    public class BehaviourTreeNodeView : Node
    {
        public BehaviourTreeNode BehaviourTreeNode { get; }

        // action 
        public Action<BehaviourTreeNode> OnNodeSelected;
        // ports
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        public BehaviourTreeNodeView(BehaviourTreeNode treeNode) : base(AssetPathResolver.GetTreeNodeUXMLPath())
        {
            BehaviourTreeNode = treeNode;
            base.title = treeNode.NodeName;

            viewDataKey = treeNode.Guid;
            style.left = treeNode.Position.x;
            style.top = treeNode.Position.y;

            // handle input / output ports
            CreateInputPorts();
            CreateOutputPorts();

            // style
            AddStyle(treeNode);

            // description 
            var description = this.Q<Label>("description");
            description.text = treeNode.NodeDescription;
        }

        private void CreateInputPorts()
        {
            switch (BehaviourTreeNode)
            {
                case LeafNode:
                case CompositeNode:
                case DecoratorNode:
                    InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (InputPort == null)
            {
                return;
            }
            InputPort.portName = "";
            InputPort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(InputPort);
        }

        private void CreateOutputPorts()
        {
            switch (BehaviourTreeNode)
            {
                case DecoratorNode:
                case RootNode:
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
                case CompositeNode:
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
            }

            if (OutputPort == null)
            {
                return;
            }
            OutputPort.portName = "";
            OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(OutputPort);
        }

        // Add style class to view that can be changed and apply style to
        private void AddStyle(BehaviourTreeNode treeNode)
        {
            switch (treeNode)
            {
                case CompositeNode:
                    AddToClassList("composite");
                    break;
                case DecoratorNode:
                    AddToClassList("decorator");
                    break;
                case RootNode:
                    AddToClassList("root");
                    break;
                case LeafNode:
                    AddToClassList("leaf");
                    break;
            }
        }

        /// <summary>
        /// Sort children
        /// </summary>
        public void SortChildren()
        {
            if (BehaviourTreeNode is CompositeNode compositeNode)
            {
                compositeNode.SortChildren();
            }
        }

        // Keep stuff in same place 
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(BehaviourTreeNode, "Set Position");
            BehaviourTreeNode.Position.x = newPos.xMin;
            BehaviourTreeNode.Position.y = newPos.yMin;
            EditorUtility.SetDirty(BehaviourTreeNode);
        }

        // when node selected 
        // propagate upward to show inspector view
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(BehaviourTreeNode);
        }

        public void UpdateNodeView()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");

            switch (BehaviourTreeNode.State)
            {

                case NodeState.Running:
                    if (BehaviourTreeNode.IsStarted)
                    {
                        AddToClassList("running");
                    }
                    break;
                case NodeState.Success:
                    AddToClassList("success");
                    break;
                case NodeState.Failure:
                    AddToClassList("failure");
                    break;
            }
        }
    }
}
