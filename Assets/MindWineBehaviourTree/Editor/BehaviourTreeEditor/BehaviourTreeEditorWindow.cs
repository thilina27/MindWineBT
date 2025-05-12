using BT.Runtime.Runner;
using BT.Runtime.Tree;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.BehaviourTreeEditor
{
    public class BehaviourTreeEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [SerializeField]
        private StyleSheet _styleSheet;

        private BehaviourTreeVew _behaviourTreeVew;
        private InspectorView _inspectorView;

        //blackboard
        private IMGUIContainer _blackboardContainer;
        private SerializedObject _serializedTree;
        private SerializedProperty _blackboardProperty;

        // filed to set tree SO
        private ObjectField _behaviourTreeField;

        [MenuItem("BehaviourTreeEditor/BehaviourTreeEditorWindow")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditorWindow");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (Selection.activeObject is not BehaviourTree)
            {
                return false;
            }

            ShowWindow();
            return true;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Instantiate UXML
            _visualTreeAsset.CloneTree(root);

            // add style
            root.styleSheets.Add(_styleSheet);

            // get two views reside inside 
            _behaviourTreeVew = root.Q<BehaviourTreeVew>();

            _inspectorView = root.Q<InspectorView>();

            // get object field and add callbacks 
            _behaviourTreeField = root.Q<ObjectField>("TreeAsset");
            _behaviourTreeField?.RegisterValueChangedCallback(OnTreeChanged);

            // blackboard
            _blackboardContainer = root.Q<IMGUIContainer>();
            _blackboardContainer.onGUIHandler = () =>
            {
                if (_serializedTree == null)
                {
                    return;
                }

                if (_serializedTree.targetObject == null)
                {
                    return;
                }

                // do update to reflect any changes done via code
                _serializedTree.Update();
                EditorGUILayout.PropertyField(_blackboardProperty);
                // apply any change done via ui to the property
                _serializedTree.ApplyModifiedProperties();
            };

            _behaviourTreeVew.OnNodeSelected = node => _inspectorView.CreateInspectorVew(node);
        }
        private void OnTreeChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue is BehaviourTree selectedTree)
            {
                if (selectedTree && AssetDatabase.CanOpenAssetInEditor(selectedTree.GetInstanceID()))
                {
                    ShowTreeView(selectedTree);
                }
            }
        }

        private void OnFocus()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnLostFocus()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            _behaviourTreeVew?.UndoRedoPerformed();
        }

        // handle play mode changes
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
        }

        private void OnPlayModeChange(PlayModeStateChange mode)
        {
            switch (mode)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    //OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        /// <summary>
        /// When we select different object
        /// </summary>
        private void OnSelectionChange()
        {
            var selectedTree = Selection.activeObject as BehaviourTree;

            if (!selectedTree && Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<BehaviourTreeRunner>(out var treeRunner))
            {
                _behaviourTreeField?.SetValueWithoutNotify(treeRunner.Tree);
                ShowTreeView(treeRunner.Tree);
                return;
            }

            if (selectedTree && AssetDatabase.CanOpenAssetInEditor(selectedTree.GetInstanceID()))
            {
                _behaviourTreeField?.SetValueWithoutNotify(selectedTree);
                ShowTreeView(selectedTree);
            }
        }

        private void ShowTreeView(BehaviourTree tree)
        {
            if (_behaviourTreeVew == null)
            {
                return;
            }
            
            _behaviourTreeVew.PopulateView(tree);
            if (tree != null)
            {
                _serializedTree = new SerializedObject(tree);
                _blackboardProperty = _serializedTree.FindProperty("_blackboard");
            }
        }

        private void OnInspectorUpdate()
        {
            _behaviourTreeVew?.UpdateTreeView();
        }
    }
}
