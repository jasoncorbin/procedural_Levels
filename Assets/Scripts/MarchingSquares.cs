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
        Debug.Log("CreateLevelGeometry called, processing tiles");

        if (tilemap == null)
        {
            tilemap = FindFirstObjectByType<Tilemap>();
        }

        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in scene!");
            return;
        }

        if (levelTexture == null)
        {
            Debug.LogError("MarchingSquares: levelTexture is not assigned!");
            return;
        }

        tilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0f);

        // Scale the Grid cell size so each tile occupies (scale x scale) world units.
        // Tile cell coords stay at (x, y, 0) — no gaps, no coordinate changes needed.
        int scale = SharedLevelData.Instance.Scale;
        Grid grid = tilemap.GetComponentInParent<Grid>();
        if (grid != null)
            grid.cellSize = new Vector3(scale, scale, 0f);
        else
            Debug.LogWarning("MarchingSquares: no parent Grid found, scale ignored.");

        tilemap.ClearAllTiles();
        TextureBasedLevel level = new TextureBasedLevel(levelTexture);
        Debug.Log($"MarchingSquares: level size {level.Width}x{level.Length}, iterating {(level.Width - 1) * (level.Length - 1)} cells");

        int debugLogCount = 0;
        int tilesPlaced = 0;

        for (int y = 0; y < level.Length - 1; y++)
        {
            for (int x = 0; x < level.Width - 1; x++)
            {
                int tL = level.IsBlocked(x,     y + 1) ? 1 : 0;
                int tR = level.IsBlocked(x + 1, y + 1) ? 1 : 0;
                int bL = level.IsBlocked(x,     y)     ? 1 : 0;
                int bR = level.IsBlocked(x + 1, y)     ? 1 : 0;
                int tileIndex = tL + tR * 2 + bL * 4 + bR * 8;

                if (debugLogCount < 20 && tileIndex > 0 && tileIndex < 15)
                {
                    Debug.Log($"[MarchingSquares] cell({x},{y}) index={tileIndex}  " +
                              $"tL={tL} tR={tR} bL={bL} bR={bR}");
                    debugLogCount++;
                }

                TileBase tile = tileset.GameTile(tileIndex);
                if (tile == null) { continue; }
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                tilesPlaced++;
            }
        }

        Debug.Log("Total tiles processed: " + tilesPlaced);
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
