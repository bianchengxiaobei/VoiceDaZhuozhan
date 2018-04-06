using UnityEngine;
using System.Collections.Generic;
using System;
/*----------------------------------------------------------------
// 模块名：VungleAdsTool
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class VungleAdsTool : IAdsTool
{
    public string appId = "5ab23be4be056466c229a465";//得判断是否是安卓还是苹果
    public string defalutPlacementId = "DEFAULT-8966946";
    public Action<EAdsPlayFailedReason> PlayFailed
    {
        get; set;
    }

    public Action PlayFinished
    {
        get; set;
    }

    public void Init()
    {
        Vungle.init(this.appId, new string[] { defalutPlacementId });
        Vungle.onAdFinishedEvent += this.PlayEnd;
    }

    public void PlayAds(string placementID)
    {
        if (Vungle.isAdvertAvailable(placementID))
        {
            Vungle.playAd(placementID);
        }
        else
        {
            if (PlayFailed != null)
            {
                PlayFailed(EAdsPlayFailedReason.NotReady);
            }
        }
    }
    public void PlayAds()
    {
        this.PlayAds(defalutPlacementId);
    }
    private void PlayEnd(string id, AdFinishedEventArgs args)
    {
        if (args.IsCompletedView)
        {
            if (PlayFinished != null)
            {
                PlayFinished();
            }
        }
        else
        {
            if (PlayFailed != null)
            {
                PlayFailed(EAdsPlayFailedReason.NotWaitComplent);
            }
        }
    }
}
