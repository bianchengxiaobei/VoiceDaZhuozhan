using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：DataGuideTaskInfo
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataGuideTaskInfo : GameData<DataGuideTaskInfo>
{
    public static string fileName = "DataGuideTaskInfo";
    public string GuideText { get; set; }
    public string UIInfoFrame { get; set; }
    public string BtnName { get; set; }
    public Vector3 InfoPos { get; set; }
    public Vector3 BtnPos { get; set; }
    public string GuideEffectPath { get; set; }
    public int BtnTriggerType { get; set; }
    public int TaskTime { get; set; }
}
