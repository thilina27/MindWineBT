// © Thilina

using UnityEngine;

namespace BT.Runtime.Nodes
{
    /// <summary>
    /// Always sits at the top
    /// </summary>
    public class RootNode : BehaviourTreeNode
    {
        [field: SerializeField] public BehaviourTreeNode Child { get; private set; }
        protected override void OnStart()
        {
        }
        protected override NodeState OnUpdate()
        {
            return Child.UpdateNode();
        }
        protected override void OnStop()
        {
        }

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
