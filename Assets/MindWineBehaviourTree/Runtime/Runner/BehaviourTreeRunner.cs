// © Thilina

using BT.Runtime.Tree;
using UnityEngine;

namespace BT.Runtime.Runner
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree _tree;

        public BehaviourTree Tree => _tree;
        protected virtual void Awake()
        {
            _tree = _tree.Clone(); 
            _tree.Bind();
        }

        protected virtual void Update()
        {
            _tree.UpdateTree();
            _tree.SetDeltaTime(Time.deltaTime);
        }
    }
}
