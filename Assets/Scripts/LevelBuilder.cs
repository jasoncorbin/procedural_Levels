using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelBuilder : MonoBehaviour
{
    [SerializeField] LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] MarchingSquares marchingSquares;
    [SerializeField] RoomDecorator roomDecorator;
    [SerializeField] LevelSaveManager levelSaveManager;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;

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
        Vector3Int centerCell = new(
            Mathf.RoundToInt(roomCenter.x),
            Mathf.RoundToInt(roomCenter.y),
            0
        );

        int maxRadius = Mathf.Max(bounds.size.x, bounds.size.y);

        for (int radius = 0; radius <= maxRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius) continue;

                    Vector3Int cell = new(centerCell.x + dx, centerCell.y + dy, 0);
                    TileBase tile = tilemap.GetTile(cell);

                    if (tile == floorTile)
                    {
                        Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
                        Debug.Log($"LevelBuilder: floor cell found at {cell}, world {worldPos}");
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
        Debug.LogError($"LevelBuilder: no floor tile found near {centerCell}. Is floorTile assigned? Using tilemap centre {fallback}.");
        return fallback;
    }

    IEnumerator SpawnPlayerDelayed(GameObject player, Rigidbody2D rb, Vector3 position)
    {
        RigidbodyType2D originalType = RigidbodyType2D.Dynamic;
        if (rb != null)
        {
            originalType = rb.bodyType;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        player.transform.position = position;

        yield return SpawnDelay;

        if (rb != null)
            rb.bodyType = originalType;
    }
}
