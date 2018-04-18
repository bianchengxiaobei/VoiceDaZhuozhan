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
        string content = (Resources.Load("levels") as TextAsset).text;
        if (string.IsNullOrEmpty(content))
        {
            Debug.LogError("content == null");
            return;
        }
        LevelConfig config = JsonConvert.DeserializeObject<LevelConfig>(content);
        foreach (var level in config.levels)
        {
            this.AddLevel(level);
        }
        if (!PlayerPrefs.HasKey(CommonDefineBase.LockedLevel))
        {
            PlayerPrefs.SetString(CommonDefineBase.LockedLevel, "1");
        }
        string[] levelsContent = PlayerPrefs.GetString(CommonDefineBase.LockedLevel).Split(',');
        foreach (var id in levelsContent)
        {
            int levelId = int.Parse(id);
            if (this.dicLevels.ContainsKey(levelId))
            {
                this.dicLevels[levelId].valid = true;
            }
        }
    }
    /// <summary>
    /// 解锁下一关
    /// </summary>
    public bool LockedNextLevel()
    {
        int nextId = this.currLevelId + 1;
        if (!this.dicLevels.ContainsKey(nextId))
        {
            //说明已经是超出最后一关
            Debug.LogError("超出最后一关");
            return false;
        }
        this.currLevelId++;
        if (!this.dicLevels[this.currLevelId].valid)
        {
            string locked = PlayerPrefs.GetString(CommonDefineBase.LockedLevel);
            locked += "," + nextId.ToString();
            PlayerPrefs.SetString(CommonDefineBase.LockedLevel, locked);
            Level level = dicLevels[nextId];
            level.valid = true;
        }
        return true;
    }
    public Level GetCurLevel()
    {
        return this.GetLevel(this.currLevelId);
    }
    public Level GetLevel(int levelId)
    {
        if (levelId > 0 && this.dicLevels.ContainsKey(levelId))
        {
            return this.dicLevels[levelId];
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
    public float GetRandomSpawTime()
    {
        return Random.Range(minSpaw, maxSpaw);
    }
    //public void EndWave(Level level)
    //{
    //    this.curWaveIndex++;
    //    if (this.curWaveIndex == level.waves.Count)
    //    {
    //        return;
    //    }
    //    PlayerManager.singleton.EndWave();
    //}
    public void Clear()
    {
        this.curWaveIndex = int.MinValue;
    }
}
