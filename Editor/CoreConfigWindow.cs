using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AmoyFeels.ProjectInitialization.Editor
{

    public class CoreConfigWindow : EditorWindow
    {
        private CoreConfig _coreConfig;
        [SerializeField] private bool InitializeOnAwake = true;
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/AmoyFeels/Project Initialization")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            CoreConfigWindow window = (CoreConfigWindow)EditorWindow.GetWindow<CoreConfigWindow>("Project Initialization");
            window.Show();
            if (window._coreConfig != null)
                return;
            var configGUID = AssetDatabase.FindAssets("t:CoreConfig").FirstOrDefault();
            var configPath = AssetDatabase.GUIDToAssetPath(configGUID);
            var config = AssetDatabase.LoadAssetAtPath(configPath, typeof(CoreConfig)) as CoreConfig;
            window._coreConfig = config;
        }

        private void OnGUI()
        {
            InitializeOnAwake = EditorGUILayout.Toggle("Initialize On Awake", InitializeOnAwake);
            if (_coreConfig != null && InitializeOnAwake != _coreConfig.InitializeOnAwake)
            {
                _coreConfig.InitializeOnAwake = InitializeOnAwake;
                EditorUtility.SetDirty(_coreConfig);
            }
        }
    }

}