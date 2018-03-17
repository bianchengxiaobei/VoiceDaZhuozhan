using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：DataGuideTipInfo
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataGuideTipInfo : GameData<DataGuideTipInfo>
{
    public static string fileName = "DataGuideTipInfo";
    public string Content { get; set; }
    public string TipPath { get; set; }
    public Vector3 LabelPos { get; set; }
}
