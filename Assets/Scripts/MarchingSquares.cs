using UnityEngine;
using UnityEngine.Tilemaps;

public class MarchingSquares : MonoBehaviour
{
    [SerializeField] Texture2D levelTexture;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tileset tileset;

    [SerializeField] LayoutGeneratorRooms layoutGenerator;

    public Tilemap Tilemap => tilemap;

    // generatedLevel is optional. When provided, room corners are chamfered before tile placement.
    [ContextMenu("Create Level Geometry")]
    public void CreateLevelGeometry(Level generatedLevel = null)
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

        // Scale the Grid cell size so each tile occupies (scale x scale) world units.
        // Tile cell coords stay at (x, y, 0) — no gaps, no coordinate changes needed.
        int scale = SharedLevelData.Instance.Scale;
        Grid grid = tilemap.GetComponentInParent<Grid>();
        if (grid != null)
            grid.cellSize = new Vector3(scale, scale, 0f);
        else
            Debug.LogWarning("MarchingSquares: no parent Grid found, scale ignored.");

        tilemap.ClearAllTiles();

        TextureBasedLevel textureLevel = new TextureBasedLevel(levelTexture);
        int width  = textureLevel.Width;
        int length = textureLevel.Length;

        // Copy texture data into a writable bool array so we can post-process it
        // without touching LayoutGeneratorRooms or the source texture.
        bool[,] blocked = new bool[width, length];
        for (int y = 0; y < length; y++)
            for (int x = 0; x < width; x++)
                blocked[x, y] = textureLevel.IsBlocked(x, y);

        // Chamfer: mark room corners as blocked to produce octagonal, organic-looking rooms.
        // LayoutGeneratorRooms.cs is not touched — all modifications are on the bool overlay only.
        if (generatedLevel != null)
        {
            foreach (Room room in generatedLevel.Rooms)
            {
                RectInt a = room.Area;
                int x0 = a.x, x1 = a.x + a.width  - 1;
                int y0 = a.y, y1 = a.y + a.height - 1;

                // 1-cell corner cut (all rooms)
                SetBlocked(blocked, width, length, x0, y0);
                SetBlocked(blocked, width, length, x1, y0);
                SetBlocked(blocked, width, length, x0, y1);
                SetBlocked(blocked, width, length, x1, y1);

                // 2-cell deep chamfer (rooms wider and taller than 4 tiles)
                if (a.width > 4 && a.height > 4)
                {
                    // top-left
                    SetBlocked(blocked, width, length, x0 + 1, y0);
                    SetBlocked(blocked, width, length, x0,     y0 + 1);
                    // top-right
                    SetBlocked(blocked, width, length, x1 - 1, y0);
                    SetBlocked(blocked, width, length, x1,     y0 + 1);
                    // bottom-left
                    SetBlocked(blocked, width, length, x0,     y1 - 1);
                    SetBlocked(blocked, width, length, x0 + 1, y1);
                    // bottom-right
                    SetBlocked(blocked, width, length, x1,     y1 - 1);
                    SetBlocked(blocked, width, length, x1 - 1, y1);
                }
            }
        }

        Debug.Log($"MarchingSquares: level size {width}x{length}, iterating {(width - 1) * (length - 1)} cells");

        int debugLogCount = 0;
        int tilesPlaced = 0;

        for (int y = 0; y < length - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int tL = blocked[x,     y + 1] ? 1 : 0;
                int tR = blocked[x + 1, y + 1] ? 1 : 0;
                int bL = blocked[x,     y]     ? 1 : 0;
                int bR = blocked[x + 1, y]     ? 1 : 0;
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

    // Bounds-checked helper to mark a cell as blocked in the overlay array.
    static void SetBlocked(bool[,] blocked, int width, int length, int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < length)
            blocked[x, y] = true;
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