using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game Configs/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Serializable]
    public class LevelData
    {
        public int level;
        public int xpToNext;
        public int coinsReward;
        public int crystalsReward;
    }

    public List<LevelData> levels = new List<LevelData>();

    public Dictionary<int, LevelData> GetLevelDictionary()
    {
        Dictionary<int, LevelData> levelDict = new Dictionary<int, LevelData>();
        foreach (var level in levels)
        {
            levelDict[level.level] = level;
        }
        return levelDict;
    }
}