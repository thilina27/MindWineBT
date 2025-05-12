// © Thilina

using BT.Runtime.Tree;
using UnityEngine;

namespace BT.Runtime.Runner
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree _tree;

        public BehaviourTree Tree => _tree;
        private void Awake()
        {
            // if we need a custom blackboard set using 
            // _tree.SetBlackboard(blackboard);
            _tree = _tree.Clone();
            _tree.Bind();
        }

        private void Update()
        {
            _tree.UpdateTree();
            _tree.SetDeltaTime(Time.deltaTime);
        }
    }
}
