using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yuddham.Editor
{
    [CustomEditor(typeof(CardData))]
    public class ardDataEditor : UnityEditor.Editor
    {
        private CardData prefabData;
        private Scene editingScene;
        private string editingScenePath = "Assets/EditingScene.unity";
        private bool sceneLoaded = false;

        /*[MenuItem("Window/Prefab Position Editor")]
    public static void ShowWindow()
    {
        GetWindow<PrefabPositionEditor>("Prefab Position Editor");
    }*/

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }


        public override void OnInspectorGUI()
        {
            //GUILayout.Label("Prefab Position Editor", EditorStyles.boldLabel);

            //prefabData = (CardData)EditorGUILayout.ObjectField("Prefab Data", prefabData, typeof(CardData), false);
 
            var activeScene = SceneManager.GetActiveScene();
            sceneLoaded = activeScene.path == editingScenePath;
            if (sceneLoaded)
            {
                editingScene = activeScene;
            }
            
            DrawDefaultInspector();

            // Add a space
            GUILayout.Space(10);

            // Get the target object
            prefabData = (CardData)target;
            if (prefabData == null)
            {
                EditorGUILayout.HelpBox("No Prefab Data loaded.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Open Editing Scene"))
            {
                OpenEditingScene();
            }

            if (sceneLoaded && GUILayout.Button("Save Changes") )
            {
                SavePrefabPositions();
                EditorUtility.SetDirty(prefabData);
                AssetDatabase.SaveAssets();
            }

            if (sceneLoaded && GUILayout.Button("Unload Editing Scene"))
            {
                UnloadEditingScene();
            }
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            //sceneLoaded = arg0.path == editingScenePath || arg1.path == editingScenePath;
Debug.Log(SceneManager.GetActiveScene());
        }

        private void OpenEditingScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Scene existingScene = EditorSceneManager.GetSceneByPath(editingScenePath);
                if (!existingScene.IsValid())
                {
                    editingScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    EditorSceneManager.SaveScene(editingScene, editingScenePath);
                }
                else
                {
                    editingScene = EditorSceneManager.OpenScene(editingScenePath, OpenSceneMode.Additive);
                }

                LoadPrefabsIntoScene();
                sceneLoaded = true;
            }
        }

        private void LoadPrefabsIntoScene()
        {
            for (var index = 0; index < prefabData.placeablesData.Length; index++)
            {
                var entry = prefabData.placeablesData[index];
                if (entry.associatedPrefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(entry.associatedPrefab);
                    instance.name = entry.associatedPrefab.name + " - " + index;
                    instance.transform.position = prefabData.relativeOffsets[index];
                }
            }
        }

        private void SavePrefabPositions()
        {
            for (var index = 0; index < prefabData.placeablesData.Length; index++)
            {
                var entry = prefabData.placeablesData[index];
                if (entry.associatedPrefab != null)
                {
                    GameObject instance = GameObject.Find(entry.associatedPrefab.name + " - "+index);
                    if (instance != null)
                    {
                        prefabData.relativeOffsets[index] = instance.transform.position;
                    }
                }
            }
        }

        private void UnloadEditingScene()
        {
            if (sceneLoaded)
            {
                EditorSceneManager.CloseScene(editingScene, true);
                sceneLoaded = false;
            }
        }
    }
}
