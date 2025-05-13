// © Thilina

namespace BT.Runtime.Nodes.CompositeNodes
{
    /// <summary>
    /// This run its children. Behave like a OR node.
    /// This return success if any child success, fail if all child fails.
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
            var childState = Children[_currentChildIndex].UpdateNode();

            switch (childState)
            {
                case NodeState.Running:
                    State = NodeState.Running;
                    break;
                case NodeState.Success:
                    State = NodeState.Success;
                    break;
                case NodeState.Failure:
                    _currentChildIndex++;
                    State = _currentChildIndex == Children.Count ? NodeState.Failure : NodeState.Running;
                    break;
            }

            return State;
        }
        protected override void OnStop()
        {
        }
    }
}
