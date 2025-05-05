// © Thilina

namespace BT.Runtime.Nodes.CompositeNodes
{
    /// <summary>
    /// This run its chits regardless of the status of each child
    /// Fail if all children fails
    /// </summary>
    public class SelectorNode : CompositeNode
    {

        private int _currentChildIndex;
        protected override void OnStart()
        {
            _currentChildIndex = 0;
        }
        protected override NodeState OnUpdate()
        {
            if (Children[_currentChildIndex].UpdateNode() != NodeState.Running)
            {
                _currentChildIndex++;
            }

            State = _currentChildIndex == Children.Count ? NodeState.Success : NodeState.Running;
            return State;
        }
        protected override void OnStop()
        {
        }
    }
}
