// © Thilina

using BT.Runtime.Attributes;
using BT.Runtime.Nodes;
using BT.Runtime.Nodes.LeafNodes;
using UnityEngine;

namespace BT.Runtime.Sample
{
    [CreateNodeMenu("Samples", "Always fail node")]
    public class FailNode : LeafNode
    {
        public override string NodeName => "Fail node";

        protected override void OnStart()
        {
        }
        protected override NodeState OnUpdate()
        {
            return NodeState.Failure;
        }
        protected override void OnStop()
        {
        }
    }
}
