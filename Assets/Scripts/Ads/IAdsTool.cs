using UnityEngine;
using System.Collections.Generic;
using System;
/*----------------------------------------------------------------
// 模块名：IAdsTool
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public interface IAdsTool
{
    Action PlayFinished { get; set; }
    Action<EAdsPlayFailedReason> PlayFailed { get; set; }

    void Init();
    void PlayAds(string placementID);
    void PlayAds();
}
