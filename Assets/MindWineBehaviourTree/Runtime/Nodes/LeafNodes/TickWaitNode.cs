// © Thilina

using UnityEngine;

namespace BT.Runtime.Nodes.LeafNodes
{
    public class TickWaitNode : LeafNode
    {
        [SerializeField] private int _waitForTicks;

        private float _currentTicks;
        public void SetWaitForTicks(int waitForTicks)
        {
            _waitForTicks = waitForTicks;
        }

        protected override void OnStart()
        {
            _currentTicks = 0;
        }

        protected override NodeState OnUpdate()
        {
            if (_currentTicks >= _waitForTicks)
            {
                return NodeState.Success;
            }
            _currentTicks++;
            return NodeState.Running;
        }

        protected override void OnStop()
        {
        }
    }
}
