using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：LevelConfig
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LevelConfig
{
    public Format exprotFormat = Format.Json;
    public List<Level> levels = new List<Level>();
}
[System.Serializable]
public class Level
{
    public int levelId;
    public bool valid = true;
    public string levelName = string.Empty;
    public string levelPath = string.Empty;
    public List<Wave> waves = new List<Wave>();
}
[System.Serializable]
public class Wave
{
    public List<MasterWave> entitys = new List<MasterWave>();
    public float duration;
    public string name;
    public int index;
}
[System.Serializable]
public class MasterWave
{
    public EntityType type;
    public float duration;
    public float speed;
    public int maxHp;
    public int demage;
    public int num;
}