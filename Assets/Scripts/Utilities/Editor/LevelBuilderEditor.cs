/*using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilder builder = (LevelBuilder)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Random Level"))
        {
            builder.GenerateRandom();
        }

        if (GUILayout.Button("Generate Level (Current Seed)"))

        {
            builder.Generate();
        }
    }
}*/
// LevelBuilderEditor.cs
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var builder = (LevelBuilder)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Random Level"))
        {
            EditorApplication.delayCall += () =>
            {
                if (builder == null) return;

                Undo.RecordObject(builder, "Generate Random Level");
                builder.GenerateRandom();

                EditorUtility.SetDirty(builder);
                EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                SceneView.RepaintAll();
            };

            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Generate Level (Current Seed)"))
        {
            EditorApplication.delayCall += () =>
            {
                if (builder == null) return;

                Undo.RecordObject(builder, "Generate Level");
                builder.Generate();

                EditorUtility.SetDirty(builder);
                EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                SceneView.RepaintAll();
            };

            GUIUtility.ExitGUI();
        }
    }
}
