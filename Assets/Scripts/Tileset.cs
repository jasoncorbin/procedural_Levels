using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "Custom/Procedural Generation/Tileset")]
public class Tileset : ScriptableObject
{
    [SerializeField]
    Color wallColor;
    [SerializeField]
    GameObject[] tiles = new GameObject[16];

    public Color WallColor => wallColor;

    public GameObject GameTile(int tileIndex)
    {
        if(tileIndex >= tiles.Length)
        {
            return null;
        }
        return tiles[tileIndex];
    
    }
    
}
