using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(LayoutGeneratorRooms))]

public class LevelGeneratorEditor : Editor

{

    public override void OnInspectorGUI()

    {

        DrawDefaultInspector();

        LayoutGeneratorRooms gen = (LayoutGeneratorRooms)target;

        if (GUILayout.Button("Generate Level Layout"))

        {

            gen.GenerateLayout();

        }
        if (GUILayout.Button("Generate New Seed"))
        {
            gen.GenerateNewSeed();
        }

        if (GUILayout.Button("Generate New Seed And Level"))
        {
            gen.GenerateNewSeedAndLevel();
        }

    }

}