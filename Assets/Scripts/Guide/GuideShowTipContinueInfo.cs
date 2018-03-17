using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：GuideShowTipContinueInfo
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataGuideShowTipContinueInfo : GameData<DataGuideShowTipContinueInfo>
{
    public static string fileName = "DataGuideShowTipContinueInfo";
    public string TipBtnPath { get; set; }
    public Vector3 TipBtnPos { get; set; }
    public string SpriteName { get; set; }
    public Vector3 SpritePos { get; set; }
    public string TipContent { get; set; }
    public int NextId { get; set; }

}
