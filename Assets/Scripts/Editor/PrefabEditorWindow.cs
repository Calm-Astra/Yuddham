using UnityEditor;
using UnityEngine;
using UnityRoyale;

public class PrefabEditorWindow : EditorWindow
{
    private static CardData currentPrefabEntry;
    private GameObject previewInstance;

    public static void ShowWindow(CardData prefabEntry)
    {
        currentPrefabEntry = prefabEntry;
        var window = GetWindow<PrefabEditorWindow>("Prefab Editor");
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        if (currentPrefabEntry != null && currentPrefabEntry.placeablesData[0].associatedPrefab != null)
        {
            previewInstance = Instantiate(currentPrefabEntry.placeablesData[0].associatedPrefab);
            previewInstance.transform.position = currentPrefabEntry.relativeOffsets[0];
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
        }
    }

    private void OnGUI()
    {
        if (currentPrefabEntry == null || currentPrefabEntry.placeablesData[0].associatedPrefab == null)
        {
            EditorGUILayout.HelpBox("No Prefab Loaded.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("Use the Move Tool to change the prefab position.");
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (previewInstance == null)
            return;

        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(previewInstance.transform.position, previewInstance.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(previewInstance.transform, "Move Prefab");
            previewInstance.transform.position = newPosition;
            currentPrefabEntry.relativeOffsets[0] = newPosition;
            EditorUtility.SetDirty(currentPrefabEntry);
        }

        sceneView.Repaint();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}