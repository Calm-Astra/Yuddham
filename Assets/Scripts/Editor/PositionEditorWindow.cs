using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRoyale;

public class PrefabPositionEditor : EditorWindow
{
    private CardData prefabData;
    private Scene editingScene;
    private string editingScenePath = "Assets/EditingScene.unity";
    private bool sceneLoaded = false;

    [MenuItem("Window/Prefab Position Editor")]
    public static void ShowWindow()
    {
        GetWindow<PrefabPositionEditor>("Prefab Position Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Position Editor", EditorStyles.boldLabel);

        prefabData = (CardData)EditorGUILayout.ObjectField("Prefab Data", prefabData, typeof(CardData), false);

        if (prefabData == null)
        {
            EditorGUILayout.HelpBox("No Prefab Data loaded.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Open Editing Scene"))
        {
            OpenEditingScene();
        }

        if (sceneLoaded && GUILayout.Button("Save Changes"))
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
