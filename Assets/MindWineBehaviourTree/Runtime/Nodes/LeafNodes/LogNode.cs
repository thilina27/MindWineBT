// © Thilina

using BT.Runtime.Attributes;
using UnityEngine;

namespace BT.Runtime.Nodes.LeafNodes
{
    [CreateNodeMenu(itemName: "Log node")]
    public class LogNode : LeafNode
    {
        [SerializeField] private string _message;

        public override string NodeName => "Log Node";

        public void SetMessage(string message)
        {
            _message = message;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnStart()
        {
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override NodeState OnUpdate()
        {
            Debug.Log(_message);
            return NodeState.Success;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnStop()
        {
        }
    }
}
