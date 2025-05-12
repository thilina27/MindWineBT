// © Thilina

using System;
using System.Collections.Generic;
using BT.Runtime.Nodes;
using BT.Runtime.Nodes.CompositeNodes;
using BT.Runtime.Nodes.DecoratorNodes;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime.Tree
{
    [CreateAssetMenu(menuName = "BT/Behaviour Tree", fileName = "Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        [field: SerializeField] public NodeState State { get; private set; }
        [field: SerializeField] public BehaviourTreeNode RootNode { get; private set; }
        [field: SerializeField] public List<BehaviourTreeNode> Nodes { get; private set; } = new();

        [SerializeField] private Blackboard _blackboard = new();

        /// <summary>
        /// Setup blackboard to the tree
        /// </summary>
        /// <param name="blackboard"></param>
        public void SetBlackboard(Blackboard blackboard)
        {
            _blackboard = blackboard;
        }
        
        /// <summary>
        /// Clone tree
        /// </summary>
        /// <returns></returns>
        public BehaviourTree Clone()
        {
            var clone = Instantiate(this);
            clone.RootNode = RootNode.Clone();
            clone.Nodes = new List<BehaviourTreeNode>();
            Traverse(clone.RootNode, node => clone.Nodes.Add(node));
            return clone;
        }

        /// <summary>
        /// Bind blackboard to nodes.
        /// Bind any outside data to nodes
        /// Modify this accordingly
        /// </summary>
        public virtual void Bind()
        {
            Traverse(RootNode, node => node.Blackboard = _blackboard);
        }
        
        /// <summary>
        /// Update tree
        /// </summary>
        /// <returns></returns>
        public NodeState UpdateTree()
        {
            if (State is NodeState.Running)
            {
                State = RootNode.UpdateNode();
            }

            return State is NodeState.Running ? State : NodeState.Success;
        }

        protected void Traverse(BehaviourTreeNode node, Action<BehaviourTreeNode> visitedCallback)
        {
            visitedCallback?.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(child => Traverse(child, visitedCallback));
        }

        #region Editor Helpers

        /// <summary>
        /// Get all children of given parent
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<BehaviourTreeNode> GetChildren(BehaviourTreeNode parent)
        {
            switch (parent)
            {
                case CompositeNode composite:
                    return composite.Children;
                case DecoratorNode decorator when decorator.Child != null:
                    return new List<BehaviourTreeNode>
                    {
                        decorator.Child
                    };
                case RootNode root when root.Child != null:
                    return new List<BehaviourTreeNode>
                    {
                        root.Child
                    };
                default:
                    return new List<BehaviourTreeNode>();
            }
        }

        /// <summary>
        /// Sample blackboard use from outside of tree
        /// </summary>
        /// <param name="deltaTime"></param>
        public void SetDeltaTime(float deltaTime)
        {
            _blackboard.DeltaTime = deltaTime;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Create a node based on the given node type
        /// </summary>
        /// <param name="nodeType"></param>
        public BehaviourTreeNode CreateNode(Type nodeType)
        {
            var node = CreateInstance(nodeType) as BehaviourTreeNode;
            if (node != null)
            {
                node.name = nodeType.Name;
                node.Guid = Guid.NewGuid().ToString();
                Nodes.Add(node);

                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError($"Cannot create node of type {nodeType}");
            }

            return node;
        }

        /// <summary>
        /// Remove a given node from the list
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(BehaviourTreeNode node)
        {
            Nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add child node to a given parent node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void AddChild(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            switch (parent)
            {
                case CompositeNode composite:
                    Undo.RecordObject(composite, "Add composite child");
                    composite.AddChild(child);
                    break;
                case DecoratorNode decorator:
                    Undo.RecordObject(decorator, "Add decorator child");
                    decorator.AddChild(child);
                    break;
                case RootNode root:
                    Undo.RecordObject(root, "Add root child");
                    root.AddChild(child);
                    break;
            }

            EditorUtility.SetDirty(parent);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Remove given child from parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChild(BehaviourTreeNode parent, BehaviourTreeNode child)
        {
            switch (parent)
            {
                case CompositeNode composite:
                    Undo.RecordObject(composite, "Remove composite child");
                    composite.RemoveChild(child);
                    break;
                case DecoratorNode decorator:
                    Undo.RecordObject(decorator, "Remove decorator child");
                    decorator.RemoveChild();
                    break;
                case RootNode root:
                    Undo.RecordObject(root, "Remove root child");
                    root.RemoveChild();
                    break;
            }

            EditorUtility.SetDirty(parent);
            AssetDatabase.SaveAssets();
        }

        public void AddRoot(BehaviourTreeNode root)
        {
            RootNode = root;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

#endif

        #endregion

    }
}
