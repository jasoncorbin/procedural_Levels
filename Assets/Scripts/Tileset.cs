using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tileset", menuName = "Custom/Procedural Generation/Tileset")]
public class Tileset : ScriptableObject
{
    [SerializeField]
    Color wallColor;
    [SerializeField]
    TileVariant[] tiles = new TileVariant[16];

    public Color WallColor => wallColor;

    public TileBase GameTile(int tileIndex)
    {
        Debug.Log("GameTile called with index: " + tileIndex +
                  ", tiles length: " + tiles.Length);

        if (tileIndex >= tiles.Length)
        {
            Debug.Log("Index out of range!");
            return null;
        }

        TileBase result = tiles[tileIndex].GetRandomTile();
        Debug.Log("Returning tile: " + (result != null ? result.name : "NULL"));
        return result;
    }
}
