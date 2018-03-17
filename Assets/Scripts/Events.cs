using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：Events
// 创建者：chen
// 修改者列表：
// 创建日期：2017.7.24
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class Events : EventsBase
{
    public static readonly string DlgStartShow = "DlgStartShow";
    public static readonly string DlgStartHide = "DlgStartHide";

    public static readonly string DlgLoadingShow = "DlgLoadingShow";
    public static readonly string DlgLoadingHide = "DlgLoadingHide";
    public static readonly string DlgLoadingAllFinished = "DlgLoadingAllFinished";

    public static readonly string DlgLevelShow = "DlgLevelShow";
    public static readonly string DlgLevelHide = "DlgLevelHide";

    public static readonly string DlgMainShow = "DlgMainShow";
    public static readonly string DlgMainHide = "DlgMainHide";
    public static readonly string DlgMainShowMask = "DlgMainShowMask";

    public static readonly string DlgPlayGuideShow = "DlgPlayGuideShow";
    public static readonly string DlgPlayGuideHide = "DlgPlayGuideHide";
    public static readonly string DlgAddGuideEvent = "DlgAddGuideEvent";
    public static readonly string DlgGuideExecuteNextTask = "DlgGuideExecuteNextTask";
    public static readonly string DlgGuideChildTaskFinished = "DlgGuideChildTaskFinished";

    public static readonly string DlgTextShow = "DlgTextShow";
    public static readonly string DlgTextHide = "DlgTextHide";
    public static readonly string DlgTextEnterLearn = "DlgTextEnterLearn";
}
