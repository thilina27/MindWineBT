// © Thilina

using BT.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    public class BlackBoardView : VisualElement
    {
        /// <summary>
        /// Mark it so it will show in UI builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BlackBoardView, UxmlTraits>
        {
        }
        
        private UnityEditor.Editor _editor;

        public void CreateView(Blackboard blackboard)
        {
            Clear();
            Object.DestroyImmediate(_editor);
            if (blackboard == null)
            {
                return;
            }
            _editor = UnityEditor.Editor.CreateEditor(blackboard);
            var container = new IMGUIContainer(() =>
            {
                if (_editor.target)
                {
                    _editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
