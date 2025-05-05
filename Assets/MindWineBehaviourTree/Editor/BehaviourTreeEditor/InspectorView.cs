// © Thilina

using BT.Runtime.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    public class InspectorView : VisualElement
    {
        /// <summary>
        /// Mark it so it will show in UI builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }

        private UnityEditor.Editor _editor;

        public void CreateInspectorVew(BehaviourTreeNode node)
        {
            Clear();
            Object.DestroyImmediate(_editor);
            if (node == null)
            {
                return;
            }
            _editor = UnityEditor.Editor.CreateEditor(node);
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
