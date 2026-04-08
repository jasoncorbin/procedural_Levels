using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class FixTileColliders
{
    // Collider type values (matches Tile.ColliderType enum)
    const int None   = 0;
    const int Sprite = 1;
    const int Grid   = 2;

    const string TileFolder = "Assets/2D_Assets/Tiles/GrassField";

    [MenuItem("Tools/Fix Tile Colliders")]
    static void Run()
    {
        string[] guids = AssetDatabase.FindAssets("t:Tile", new[] { TileFolder });

        if (guids.Length == 0)
        {
            Debug.LogWarning($"FixTileColliders: no Tile assets found in {TileFolder}");
            return;
        }

        int updated = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile == null) continue;

            // Derive index from asset name: "GrassField_N" -> N
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            string suffix = fileName.Replace("GrassField_", "");
            if (!int.TryParse(suffix, out int index))
            {
                Debug.LogWarning($"FixTileColliders: could not parse index from '{fileName}', skipping.");
                continue;
            }

            int targetColliderType = index == 0  ? None
                                   : index == 15 ? Grid
                                   :               Sprite;

            // Use SerializedObject so the change is persisted correctly regardless of
            // whether the asset is a Tile or RuleTile (field names differ).
            var so = new SerializedObject(tile);

            // Plain Tile uses "m_ColliderType"; RuleTile uses "m_DefaultColliderType"
            SerializedProperty prop = so.FindProperty("m_ColliderType")
                                  ?? so.FindProperty("m_DefaultColliderType");

            if (prop == null)
            {
                Debug.LogWarning($"FixTileColliders: no collider type property found on '{fileName}'.");
                continue;
            }

            if (prop.intValue != targetColliderType)
            {
                prop.intValue = targetColliderType;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(tile);
                updated++;
                string typeName = targetColliderType == None ? "None" : targetColliderType == Grid ? "Grid" : "Sprite";
                Debug.Log($"FixTileColliders: {fileName} (index {index}) -> {typeName}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"FixTileColliders: done. {updated} tile(s) updated out of {guids.Length} found.");
    }
}