using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：DataPopTipWindowTaskInfo
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataPopTipWindowTaskInfo : GameData<DataPopTipWindowTaskInfo>
{
    public static string fileName = "DataPopTipWindowTaskInfo";
    public string Content { get; set; }
    public string Path { get; set; }
    public Vector3 Pos { get; set; }
    public Vector3 ToScale { get; set; }
    public float Duration { get; set; }
}
