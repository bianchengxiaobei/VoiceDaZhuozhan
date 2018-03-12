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
    public List<Level> levels = new List<Level>();
}

