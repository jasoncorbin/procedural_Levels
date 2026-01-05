/*using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutGeneratorRooms))]
public class LevelGeneratorEditor : Editor 
{ 
    public override void OnInspectorGUI() 
    { 
        DrawDefaultInspector(); 
        LayoutGeneratorRooms gen = (LayoutGeneratorRooms)target; 
        if (GUILayout.Button("Generate Level Layout")) 
        { 
            gen.GenerateLevel(); 

        }
        if (GUILayout.Button("Generate New Seed")) 
        { 
            gen.GenerateNewSeed(); 

        }
        if (GUILayout.Button("Generate New Seed And Level")) 
        { 
            gen.GenerateNewSeedAndLevel(); 
            
        }
    DrawDefaultInspector(); 
    } 
}
*/
// LevelGeneratorEditor.cs
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(LayoutGeneratorRooms))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gen = (LayoutGeneratorRooms)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Layout Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Level Layout"))
        {
            EditorApplication.delayCall += () =>
            {
                if (gen == null) return;

                Undo.RecordObject(gen, "Generate Level Layout");
                gen.GenerateLevel();

                EditorUtility.SetDirty(gen);
                EditorSceneManager.MarkSceneDirty(gen.gameObject.scene);
                SceneView.RepaintAll();
            };

            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Generate New Seed"))
        {
            EditorApplication.delayCall += () =>
            {
                if (gen == null) return;

                Undo.RecordObject(gen, "Generate New Seed");
                gen.GenerateNewSeed();

                EditorUtility.SetDirty(gen);
                EditorSceneManager.MarkSceneDirty(gen.gameObject.scene);
                SceneView.RepaintAll();
            };

            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Generate New Seed And Level"))
        {
            EditorApplication.delayCall += () =>
            {
                if (gen == null) return;

                Undo.RecordObject(gen, "Generate New Seed And Level");
                gen.GenerateNewSeedAndLevel();

                EditorUtility.SetDirty(gen);
                EditorSceneManager.MarkSceneDirty(gen.gameObject.scene);
                SceneView.RepaintAll();
            };

            GUIUtility.ExitGUI();
        }
    }
}
