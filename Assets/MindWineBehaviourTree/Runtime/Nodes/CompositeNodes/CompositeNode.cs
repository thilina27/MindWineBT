// © Thilina

using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime.Nodes.CompositeNodes
{
    /// <summary>
    /// This node can have one or more children.
    /// Order of execution can be determined by the type of the node
    /// </summary>
    public abstract class CompositeNode : BehaviourTreeNode
    {
        [field: SerializeField] public List<BehaviourTreeNode> Children { get; private set; } = new();

        public void AddChild(BehaviourTreeNode node)
        {
            Children.Add(node);
            SortChildren();
        }

        public void RemoveChild(BehaviourTreeNode node)
        {
            Children.Remove(node);
        }

        public override BehaviourTreeNode Clone()
        {
            var clone = Instantiate(this);
            clone.Children = Children.ConvertAll(c => c.Clone());
            return clone;
        }

        /// <summary>
        /// Sort node based on position of the graph
        /// </summary>
        public void SortChildren()
        {
            Children.Sort(SortChildrenByPosition);
        }

        private int SortChildrenByPosition(BehaviourTreeNode left, BehaviourTreeNode right)
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }
    }
}
