// © Thilina

using System;

namespace BT.Runtime.Tree
{
    /// <summary>
    /// Blackboard that will be shared with all nodes in the tree
    /// </summary>
    [Serializable]
    public class Blackboard
    {
        public float DeltaTime;
    }
}
