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
        if (variants == null || variants.Length == 0)
        {
            Debug.LogWarning("TileVariant has no variants assigned.");
            return null;
        }

        Random random = SharedLevelData.Instance != null ? SharedLevelData.Instance.Rand : null;
        if (random == null) random = new Random(Environment.TickCount);

        int randomIndex = random.Next(variants.Length);
        return variants[randomIndex];
    }
}
