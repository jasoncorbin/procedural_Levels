using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

[Serializable]
public class TileVariant
{
    [SerializeField] private TileBase[] variants = Array.Empty<TileBase>();

    public TileBase GetRandomTile()
    {
        Debug.Log("TileVariant.GetRandomTile: variants length = " +
                  (variants != null ? variants.Length.ToString() : "NULL array"));

        if (variants == null || variants.Length == 0)
        {
            Debug.LogWarning("TileVariant has no variants assigned.");
            return null;
        }

        Random random = SharedLevelData.Instance != null ? SharedLevelData.Instance.Rand : null;
        if (random == null) random = new Random(Environment.TickCount);

        int randomIndex = random.Next(variants.Length);
        TileBase result = variants[randomIndex];
        Debug.Log("TileVariant.GetRandomTile: returning index " + randomIndex +
                  " = " + (result != null ? result.name : "NULL"));
        return result;
    }
}
