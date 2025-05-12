// © Thilina

using System;
using UnityEditor;
using UnityEngine;

namespace BT.Runtime.Tree
{
    /// <summary>
    /// Blackboard that will be shared with all nodes in the tree
    /// </summary>
    [CreateAssetMenu(menuName = "BT/Blackboard")]
    public class Blackboard : ScriptableObject
    {
        public float DeltaTime;
    }
}
