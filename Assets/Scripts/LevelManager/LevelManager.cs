using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.Data;
using Newtonsoft.Json;
/*----------------------------------------------------------------
// 模块名：LevelManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    public Dictionary<int, Level> dicLevels = new Dictionary<int, Level>();
    public int currLevelId;
    public int curWaveIndex;
    public float minSpaw = 0.5f;
    public float maxSpaw = 10f;
    public void AddLevel(Level level)
    {
        if (!this.dicLevels.ContainsKey(level.levelId))
        {
            this.dicLevels.Add(level.levelId, level);
        }
    }

    public void EnterLevel(int levelId)
    {
        if (levelId > 0 && this.dicLevels.ContainsKey(levelId))
        {
            this.currLevelId = levelId;
            this.curWaveIndex = 0;
        }
    }
    public void LoadLevel()
    {
        string content = UnityTool.LoadTxtFile(Application.dataPath + "/Resources/levels.txt");
        LevelConfig config = JsonConvert.DeserializeObject<LevelConfig>(content);
        foreach (var level in config.levels)
        {
            this.AddLevel(level);
        }
    }
    public Level GetCurLevel()
    {
        if (this.currLevelId > 0 && this.dicLevels.ContainsKey(this.currLevelId))
        {
            return this.dicLevels[this.currLevelId];
        }
        return null;
    }
    public int GetCurWaveNum()
    {
        Level cur = this.GetCurLevel();
        if (cur != null)
        {
            return cur.waves.Count;
        }
        return 0;
    }
    public bool HasLockedLevel(Level level)
    {
        return true;
    }
    public float GetRandomSpawTime()
    {
        return Random.Range(minSpaw, maxSpaw);
    }
    public void EndWave(Level level)
    {
        this.curWaveIndex++;
        if (this.curWaveIndex == level.waves.Count)
        {
            return;
        }
        PlayerManager.singleton.EndWave();

    }
}
