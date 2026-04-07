using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelBuilder : MonoBehaviour
{
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] MarchingSquares marchingSquares;
    [SerializeField] RoomDecorator roomDecorator;
    [SerializeField] LevelSaveManager levelSaveManager;
    [SerializeField] Tileset tileset;

    static readonly WaitForSeconds SpawnDelay = new(0.5f);

    void Start()
    {
        GenerateRandom();
    }

    [ContextMenu("Generate Random Level")]
    public void GenerateRandom()
    {
        SharedLevelData.Instance.GenerateSeed();
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate(string levelId = null)
    {
        if (levelId != null && levelSaveManager != null)
        {
            LevelSaveData saveData = levelSaveManager.LoadLevel(levelId);
            if (saveData != null)
                levelSaveManager.ApplySeedFromSave(saveData);
        }

        Level level = layoutGeneratorRooms.GenerateLevel();
        marchingSquares.CreateLevelGeometry();
        roomDecorator.PlaceItems(level);

        Debug.Log("Scale: " + SharedLevelData.Instance.Scale);

        Tilemap tilemap = marchingSquares.Tilemap != null ? marchingSquares.Tilemap : FindFirstObjectByType<Tilemap>();
        if (tilemap != null)
        {
            Debug.Log("Tilemap cellBounds: " + tilemap.cellBounds);
            Debug.Log("Tilemap localBounds: " + tilemap.localBounds);
        }

        Room startRoom = level.playerStartRoom;
        Debug.Log("Start room area: " + startRoom.Area);

        Vector2 roomCenter = new(
            startRoom.Area.x + startRoom.Area.width / 2f,
            startRoom.Area.y + startRoom.Area.height / 2f
        );
        Debug.Log("Room center raw: " + roomCenter);

        Vector3 spawnPos = FindFloorSpawn(tilemap, roomCenter);
        Debug.Log("Final spawn position: " + spawnPos);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        StopAllCoroutines(); // prevent overlapping spawn coroutines leaving rb stuck as Kinematic
        StartCoroutine(SpawnPlayerDelayed(player, rb, spawnPos));
    }

    // Searches outward in a spiral from roomCenter for a cell whose tile equals floorTile.
    // Falls back to the exact centre of the tilemap bounds if nothing is found.
    Vector3 FindFloorSpawn(Tilemap tilemap, Vector2 roomCenter)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("LevelBuilder: no Tilemap found, using raw roomCenter.");
            return roomCenter;
        }

        BoundsInt bounds = tilemap.cellBounds;
        // Tiles are placed at (x, y, 0) matching texture coords directly.
        Vector3Int centerCell = new(
            Mathf.RoundToInt(roomCenter.x),
            Mathf.RoundToInt(roomCenter.y),
            0
        );
        Debug.Log($"LevelBuilder: spiral search start cell={centerCell}, roomCenter={roomCenter}");

        // Wall tile is index 15 in whatever tileset is currently active.
        // Comparing against the tileset's own wall reference means this works
        // regardless of which tileset (GrassField, Dungeon, etc.) is assigned.
        TileBase wallTile = tileset != null ? tileset.GameTile(15) : null;

        int maxRadius = Mathf.Max(bounds.size.x, bounds.size.y);

        for (int radius = 0; radius <= maxRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius) continue;

                    Vector3Int cell = new(centerCell.x + dx, centerCell.y + dy, 0);
                    TileBase foundTile = tilemap.GetTile(cell);

                    if (foundTile != null && foundTile != wallTile)
                    {
                        Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
                        Debug.Log($"LevelBuilder: floor cell found at {cell}, tile={foundTile.name}, world {worldPos}");
                        return worldPos;
                    }
                }
            }
        }

        // Nothing found — use tilemap bounds centre
        Vector3 fallback = tilemap.GetCellCenterWorld(new Vector3Int(
            bounds.x + bounds.size.x / 2,
            bounds.y + bounds.size.y / 2,
            0
        ));
        Debug.LogError($"LevelBuilder: no non-wall tile found near {centerCell}. Using tilemap centre {fallback}.");
        return fallback;
    }

    IEnumerator SpawnPlayerDelayed(GameObject player, Rigidbody2D rb, Vector3 position)
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            Debug.Log($"LevelBuilder: Rigidbody2D set to Kinematic, placing player at {position}");
        }

        player.transform.position = position;

        yield return SpawnDelay;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // always restore to Dynamic explicitly
            Debug.Log("LevelBuilder: Rigidbody2D restored to Dynamic");
        }
    }
}
