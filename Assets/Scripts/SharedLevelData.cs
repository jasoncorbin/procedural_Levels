using UnityEngine;
using System;
using Random = System.Random;

[ExecuteAlways]
[DisallowMultipleComponent]
public class SharedLevelData : MonoBehaviour
{
    
    public static SharedLevelData Instance { get; private set;}
    [SerializedField] int scale = 1;
    [SerializedField] int seed = Environment.TickCount;

    Random random;
    public int Scale => scale;
    public Random Rand => random;

    [ContexMenu("Generate New Seed")]
    public void GenerateNewSeed()
    {
        seed = Environment.TickCount;
        random = new Random(seed);
    }
    

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            enabled = false;
            Debug.LogWarning("duplicate SharedLevelData detected and disabled", this);
        }
            
        Debug.Log(Instance.GetInstaceID());
        
    }

    public void ResetRandom()
    {
        random = new Random(seed);
    }

}
