using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：IFlyTextManager 
// 创建者：chen
// 修改者列表：
// 创建日期：2016.4.23
// 模块描述：浮动文字管理器
//----------------------------------------------------------------*/
#endregion
/// <summary>
/// 浮动文字管理器
/// </summary>
internal interface IFlyTextManager 
{
    FlyTextEntity Add(string strText, int target);
    void Update();
}
