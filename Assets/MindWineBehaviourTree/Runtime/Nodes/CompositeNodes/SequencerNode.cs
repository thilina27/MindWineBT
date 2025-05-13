// © Thilina

namespace BT.Runtime.Nodes.CompositeNodes
{
    /// <summary>
    /// This executes each child in order, behave as an and Node.
    /// if last one is success this node is also success.
    /// If one fail this node fails
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        private int _currentChildIndex;

        protected override void OnStart()
        {
            _currentChildIndex = 0;
        }

        protected override NodeState OnUpdate()
        {
            State = Children[_currentChildIndex].UpdateNode();
            switch (State)
            {
                case NodeState.Running:
                case NodeState.Failure:
                    return State;
                case NodeState.Success:
                    _currentChildIndex++;
                    break;
            }

            State = _currentChildIndex == Children.Count ? NodeState.Success : NodeState.Running;
            return State;
        }

        protected override void OnStop()
        {
        }
    }
}
