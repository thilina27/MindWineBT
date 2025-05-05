// © Thilina

namespace BT.Runtime.Nodes.DecoratorNodes
{
    /// <summary>
    /// Repeat child forever
    /// </summary>
    public class RepeaterNode : DecoratorNode
    {
        protected override void OnStart()
        {

        }

        protected override NodeState OnUpdate()
        {
            Child.UpdateNode();
            return NodeState.Running;
        }

        protected override void OnStop()
        {
        }
    }
}
