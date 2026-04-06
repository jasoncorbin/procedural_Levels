using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class LevelSaveManager : MonoBehaviour
{
    public static LevelSaveManager Instance { get; private set; }

    string SaveDirectory => Path.Combine(Application.persistentDataPath, "levels");

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Directory.CreateDirectory(SaveDirectory);
    }

    // Serializes the current level to JSON and writes it to disk.
    // Seed is read from SharedLevelData via reflection since the field is private.
    public void SaveLevel(Level level, string levelId, string levelType)
    {
        var saveData = new LevelSaveData
        {
            levelId   = levelId,
            seed      = ReadSeedFromSharedLevelData(),
            levelType = levelType,
            placedItems = new List<ItemSaveState>()
        };

        File.WriteAllText(SavePath(levelId), JsonUtility.ToJson(saveData, true));
        Debug.Log($"LevelSaveManager: saved '{levelId}' ({levelType}) to {SavePath(levelId)}");
    }

    // Returns the deserialized LevelSaveData, or null if the file does not exist.
    public LevelSaveData LoadLevel(string levelId)
    {
        string path = SavePath(levelId);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<LevelSaveData>(File.ReadAllText(path));
    }

    public bool LevelExists(string levelId) => File.Exists(SavePath(levelId));

    // Writes the saved seed back into SharedLevelData and resets its Random instance,
    // so that subsequent generation reproduces the exact same layout.
    public void ApplySeedFromSave(LevelSaveData saveData)
    {
        FieldInfo seedField = typeof(SharedLevelData)
            .GetField("seed", BindingFlags.NonPublic | BindingFlags.Instance);

        if (seedField == null)
        {
            Debug.LogError("LevelSaveManager: could not find 'seed' field on SharedLevelData via reflection.");
            return;
        }

        seedField.SetValue(SharedLevelData.Instance, saveData.seed);
        SharedLevelData.Instance.ResetRandom();
    }

    // ---- private helpers ----

    string SavePath(string levelId) => Path.Combine(SaveDirectory, levelId + ".json");

    int ReadSeedFromSharedLevelData()
    {
        FieldInfo seedField = typeof(SharedLevelData)
            .GetField("seed", BindingFlags.NonPublic | BindingFlags.Instance);
        return seedField != null ? (int)seedField.GetValue(SharedLevelData.Instance) : 0;
    }
}
