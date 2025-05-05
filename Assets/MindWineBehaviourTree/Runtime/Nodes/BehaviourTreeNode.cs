// © Thilina

using BT.Runtime.Tree;
using UnityEngine;

namespace BT.Runtime.Nodes
{
    public abstract class BehaviourTreeNode : ScriptableObject
    {
        protected const int MaxTick = 100000;
        public NodeState State { get; protected set; }
        public bool IsStarted { get; protected set; }
        public int TickCount { get; protected set; }

        [HideInInspector] public Blackboard Blackboard;

        // override to change display name of node
        public virtual string NodeName => GetType().Name;

        // override to show description for node
        // use text area
        public virtual string NodeDescription => string.Empty;


        // used for graph editor
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;

        public NodeState UpdateNode()
        {
            if (!IsStarted)
            {
                State = NodeState.Running;
                IsStarted = true;
                OnStart();
            }

            State = OnUpdate();

            if (State is not NodeState.Running)
            {
                IsStarted = false;
                OnStop();
            }

            TickCount = (TickCount + 1) % MaxTick;
            return State;
        }

        public virtual BehaviourTreeNode Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract NodeState OnUpdate();
        protected abstract void OnStop();

    }
}
