using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Advertisements;
/*----------------------------------------------------------------
// 模块名：UnityAdsTool
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class UnityAdsTool : IAdsTool
{
    public string appId = "1742371";//得判断是否是安卓还是苹果
    public string defaultPlacementId = "rewardedVideo";
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
        if (!Advertisement.isSupported)
        {
            if (PlayFailed != null)
            {
                PlayFailed(EAdsPlayFailedReason.NotSupport);
            }
            return;
        }
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(appId);
        }
    }

    public void PlayAds(string placementID)
    {
        if (Advertisement.IsReady(placementID))
        {
            if (Advertisement.GetPlacementState() == PlacementState.Ready)
            {
                ShowOptions options = new ShowOptions();
                options.resultCallback = this.HandleCallback;
                Advertisement.Show(placementID, options);
            }
            else if (Advertisement.GetPlacementState() == PlacementState.Disabled)
            {
                if (PlayFailed != null)
                {
                    PlayFailed(EAdsPlayFailedReason.NotSupport);
                }
            }
            else
            {
                if (PlayFailed != null)
                {
                    PlayFailed(EAdsPlayFailedReason.NotReady);
                }
            }
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
        this.PlayAds(defaultPlacementId);
    }
    private void HandleCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                if (PlayFinished != null)
                {
                    PlayFinished();
                }
                break;
            case ShowResult.Skipped:
                if (PlayFailed != null)
                {
                    PlayFailed(EAdsPlayFailedReason.NotWaitComplent);
                }
                break;
            case ShowResult.Failed:
                if (PlayFailed != null)
                {
                    PlayFailed(EAdsPlayFailedReason.Error);
                }
                break;
        }
    }
}
