using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(MarchingSquares))]
public class MarchingSquaresEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var geo = (MarchingSquares)target;

        // Find the layout generator in the scene
        LayoutGeneratorRooms layout =
            Object.FindObjectOfType<LayoutGeneratorRooms>();

        GUILayout.Space(8);
        EditorGUILayout.LabelField("Geometry Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Level Geometry"))
        {
            Run(geo, layout, GeometryOnly);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Seed + Layout", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(layout == null))
        {
            if (GUILayout.Button("Generate New Seed"))
            {
                Run(geo, layout, SeedOnly);
            }

            if (GUILayout.Button("Generate New Seed And Level"))
            {
                Run(geo, layout, SeedAndLevel);
            }
        }
    }

    private void Run(
        MarchingSquares geo,
        LayoutGeneratorRooms layout,
        System.Action<MarchingSquares, LayoutGeneratorRooms> action)
    {
        var parent = GetGeneratedParent(geo);

        if (parent != null)
            Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Level Generation");

        if (layout != null)
            Undo.RecordObject(layout, "Layout Generation");

        action.Invoke(geo, layout);

        // Mark dirty + force refresh
        if (geo != null) EditorUtility.SetDirty(geo);
        if (layout != null) EditorUtility.SetDirty(layout);

        EditorSceneManager.MarkSceneDirty(geo.gameObject.scene);
        SceneView.RepaintAll();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        Repaint();
    }

    // -------- ACTIONS --------

    private void GeometryOnly(MarchingSquares geo, LayoutGeneratorRooms layout)
    {
        geo.CreateLevelGeometry();
    }

    private void SeedOnly(MarchingSquares geo, LayoutGeneratorRooms layout)
    {
        layout.GenerateNewSeed();
        geo.CreateLevelGeometry();
    }

    private void SeedAndLevel(MarchingSquares geo, LayoutGeneratorRooms layout)
    {
        layout.GenerateNewSeedAndLevel();
        geo.CreateLevelGeometry();
    }

    // -------- HELPERS --------

    private Transform GetGeneratedParent(MarchingSquares geo)
    {
        var so = new SerializedObject(geo);
        var prop = so.FindProperty("generatedLevel");
        if (prop == null) return null;

        return (prop.objectReferenceValue as GameObject)?.transform;
    }
}
