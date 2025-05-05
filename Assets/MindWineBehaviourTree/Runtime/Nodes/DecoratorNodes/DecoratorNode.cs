// © Thilina

using UnityEngine;

namespace BT.Runtime.Nodes.DecoratorNodes
{
    /// <summary>
    /// This node has only one child. it used to mutate/decorate the child.
    /// </summary>
    public abstract class DecoratorNode : BehaviourTreeNode
    {
        [field: SerializeField] public BehaviourTreeNode Child { get; private set; }

        public void AddChild(BehaviourTreeNode child)
        {
            Child = child;
        }

        public void RemoveChild()
        {
            Child = null;
        }

        public override BehaviourTreeNode Clone()
        {
            var clone = Instantiate(this);
            clone.Child = Child.Clone();
            return clone;
        }
    }
}
