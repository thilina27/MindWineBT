// © Thilina

using UnityEditor;

namespace BT.Editor.BehaviourTreeEditor
{
    public static class AssetPathResolver
    {
        private const string nodeViewXMLName = "BehaviourTreeNodeView.uxml";
        private static string _nodeViewUXMLPath;

        private const string behaviourTreeEditorUSSName = "BehaviourTreeEditorWindow.uss";
        private static string _treeEditorUSSPath;

        public static string GetTreeNodeUXMLPath()
        {
            if (string.IsNullOrEmpty(_nodeViewUXMLPath))
            {
                RefreshPaths();
            }

            return _nodeViewUXMLPath;
        }

        public static string GetTreeEditorUSSPath()
        {
            if (string.IsNullOrEmpty(_treeEditorUSSPath))
            {
                RefreshPaths();
            }
            return _treeEditorUSSPath;
        }

        private static void RefreshPaths()
        {
            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (var path in allAssetPaths)
            {
                if (path.EndsWith(nodeViewXMLName))
                {
                    _nodeViewUXMLPath = path;
                    continue;
                }
                if (path.EndsWith(behaviourTreeEditorUSSName))
                {
                    _treeEditorUSSPath = path;
                }
            }
        }

    }
}
