using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;


[Serializable]
public class TileVariant
{
    [SerializeField] private GameObject[] variants = Array.Empty<GameObject>();

    public GameObject GetRandomTile()
    {
        if (variants == null || variants.Length == 0)
        {
            Debug.LogWarning("TileVariant has no variants assigned.");
            return null;
        }
        

        Random random = SharedLevelData.Instance != null ? SharedLevelData.Instance.Rand : null;
        if (random == null) random = new Random(Environment.TickCount);

        int randomIndex = random.Next(variants.Length); // 0..Length-1
        return variants[randomIndex];
    }
    
    /*[SerializeField]GameObject[] variants = new GameObject[0];

    public GameObject GetRandomTile()
    {
        Random random = SharedLevelData.Instance.Rand;
        int randomIndex = random.Next(0, variants.Length);
        return variants[randomIndex];
    }*/



}
