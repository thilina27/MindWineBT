// © Thilina

using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    public class SplitView : TwoPaneSplitView
    {
        /// <summary>
        /// Mark it so it will show in UI builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }
    }
}
