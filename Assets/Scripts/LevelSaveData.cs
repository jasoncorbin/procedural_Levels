using System;
using System.Collections.Generic;

[Serializable]
public class LevelSaveData
{
    public string levelId;
    public int seed;
    public string levelType; // "Procedural" or "Story"
    public List<ItemSaveState> placedItems = new List<ItemSaveState>();
}
