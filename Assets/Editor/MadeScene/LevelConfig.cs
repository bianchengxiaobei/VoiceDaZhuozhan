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

public class LevelConfig : ScriptableObject
{
    public Format exprotFormat = Format.Json;
    public int SelfMaxHp;
    public int PangziMaxHp;
    public int TiaoTiaoMaxHp;
    public int WenShenMaxHp;
    public int FeijiMaxHp;
    public int WajuejiMaxHp;
    public int ShunxiMaxHp;
    public int PufuMaxHp;
    public float PangziSpeed;
    public float TiaoTiaoSpeed;
    public float WenShenSpeed;
    public float FeijiSpeed;
    public float WajuejiSpeed;
    public float ShunxiSpeed;
    public float PufuSpeed;
    public List<Level> levels = new List<Level>();
}

