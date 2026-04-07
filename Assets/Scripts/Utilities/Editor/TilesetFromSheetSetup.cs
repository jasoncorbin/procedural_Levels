using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilesetFromSheetSetup
{
    [MenuItem("Tools/Setup Tileset From Sheet")]
    static void Run()
    {
        const string spriteSheetPath  = "Assets/2D_Assets/Sprites/tileset_grassfield.png";
        const string tileOutputFolder  = "Assets/2D_Assets/Tiles/GrassField";
        const string tilesetOutputPath = "Assets/TileSets/Tileset_GrassField.asset";
        const string tilePrefix        = "GrassField_";
        const int    tileCount         = 16;

        // ── 1. Load all sprites from the sheet ────────────────────────────────
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath)
            as Sprite[] ?? System.Array.Empty<Sprite>();

        // Filter to only Sprite sub-assets and sort by name suffix index
        var spriteList = new Sprite[tileCount];
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath))
        {
            if (obj is not Sprite s) continue;
            // Expect names like "tileset_grassfield_0" … "tileset_grassfield_15"
            string suffix = s.name.Replace("tileset_grassfield_", "");
            if (int.TryParse(suffix, out int idx) && idx >= 0 && idx < tileCount)
                spriteList[idx] = s;
        }

        // Validate
        for (int i = 0; i < tileCount; i++)
        {
            if (spriteList[i] == null)
            {
                Debug.LogError($"TilesetFromSheetSetup: sprite 'tileset_grassfield_{i}' not found in {spriteSheetPath}. " +
                               "Make sure the sheet is imported and sliced with that naming convention.");
                return;
            }
        }

        // ── 2. Create output folder if needed ─────────────────────────────────
        if (!AssetDatabase.IsValidFolder(tileOutputFolder))
        {
            string parent = Path.GetDirectoryName(tileOutputFolder).Replace('\\', '/');
            string folder = Path.GetFileName(tileOutputFolder);
            AssetDatabase.CreateFolder(parent, folder);
        }

        // ── 3. Create 16 Tile assets ───────────────────────────────────────────
        var tileAssets = new Tile[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            string tilePath = $"{tileOutputFolder}/{tilePrefix}{i}.asset";

            // Reuse existing asset if already there
            Tile existing = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (existing != null)
            {
                existing.sprite = spriteList[i];
                EditorUtility.SetDirty(existing);
                tileAssets[i] = existing;
            }
            else
            {
                var tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = spriteList[i];
                AssetDatabase.CreateAsset(tile, tilePath);
                tileAssets[i] = tile;
            }
        }

        AssetDatabase.SaveAssets();

        // ── 4. Create or load the Tileset ScriptableObject ───────────────────
        Tileset tileset = AssetDatabase.LoadAssetAtPath<Tileset>(tilesetOutputPath);
        if (tileset == null)
        {
            tileset = ScriptableObject.CreateInstance<Tileset>();
            AssetDatabase.CreateAsset(tileset, tilesetOutputPath);
        }

        // ── 5. Assign tiles into TileVariant slots via SerializedObject ───────
        var so = new SerializedObject(tileset);
        SerializedProperty tilesProp = so.FindProperty("tiles");

        if (tilesProp == null)
        {
            Debug.LogError("TilesetFromSheetSetup: could not find 'tiles' property on Tileset. " +
                           "Check that the field is named 'tiles' and has [SerializeField].");
            return;
        }

        tilesProp.arraySize = tileCount;

        for (int i = 0; i < tileCount; i++)
        {
            SerializedProperty variantProp = tilesProp.GetArrayElementAtIndex(i);
            SerializedProperty variantsProp = variantProp.FindPropertyRelative("variants");

            if (variantsProp == null)
            {
                Debug.LogError("TilesetFromSheetSetup: could not find 'variants' property on TileVariant. " +
                               "Check that the field is named 'variants' and has [SerializeField].");
                return;
            }

            variantsProp.arraySize = 1;
            variantsProp.GetArrayElementAtIndex(0).objectReferenceValue = tileAssets[i];
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(tileset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"TilesetFromSheetSetup: created {tileCount} tiles in '{tileOutputFolder}' " +
                  $"and Tileset at '{tilesetOutputPath}'.");
        Selection.activeObject = tileset;
    }
}
