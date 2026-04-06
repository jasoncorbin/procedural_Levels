using UnityEngine;
using UnityEngine.Tilemaps;

public class MarchingSquares : MonoBehaviour
{
    [SerializeField] Texture2D levelTexture;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tileset tileset;

    [SerializeField] LayoutGeneratorRooms layoutGenerator;

    public Tilemap Tilemap => tilemap;

    [ContextMenu("Create Level Geometry")]
    public void CreateLevelGeometry()
    {
        if (tilemap == null)
        {
            tilemap = FindFirstObjectByType<Tilemap>();
        }

        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in scene!");
            return;
        }

        tilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0f);
        tilemap.ClearAllTiles();
        TextureBasedLevel level = new TextureBasedLevel(levelTexture);
        for (int y = 1; y < level.Length - 2; y++)
        {
            for (int x = 1; x < level.Width - 2; x++)
            {
                int tileIndex = CalculateTileIndex(level, x, y);
                TileBase tile = tileset.GameTile(tileIndex);
                if (tile == null) { continue; }
                tilemap.SetTile(new Vector3Int(x - 1, level.Length - 2 - y, 0), tile);
            }
        }
    }

    int CalculateTileIndex(ILevel level, int x, int y)
    {
        int topLeft     = level.IsBlocked(x,     y + 1) ? 1 : 0;
        int topRight    = level.IsBlocked(x + 1, y + 1) ? 1 : 0;
        int bottomLeft  = level.IsBlocked(x,     y)     ? 1 : 0;
        int bottomRight = level.IsBlocked(x + 1, y)     ? 1 : 0;
        int tileIndex = topLeft + topRight * 2 + bottomLeft * 4 + bottomRight * 8;
        return tileIndex;
    }
}
