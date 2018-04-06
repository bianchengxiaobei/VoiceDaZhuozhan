using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
/*----------------------------------------------------------------
// 模块名：AdsManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public enum EAdsType
{
    UnityAds,
    Vungle
}
public enum EAdsPlayFailedReason
{
    NotReady,
    NotWaitComplent,
    NotSupport,
    Error
}
public class AdsManager : Singleton<AdsManager>
{
    public Dictionary<EAdsType, IAdsTool> m_dicAds = new Dictionary<EAdsType, IAdsTool>();
    private int curIndex = int.MinValue;
    private int nextCount = 0;
    private int Count = 0;
    public Action playNextFailed;
    public int AdsTime;
    public void Init()
    {
        this.m_dicAds.Add(EAdsType.UnityAds,new UnityAdsTool());
        this.m_dicAds.Add(EAdsType.Vungle, new VungleAdsTool());
        foreach (var ads in this.m_dicAds)
        {
            ads.Value.Init();
        }
        this.Count = m_dicAds.Count;
    }
    public void PlayAds(EAdsType type)
    {
        this.curIndex = (int)type;
        IAdsTool ads;
        if (this.m_dicAds.TryGetValue(type, out ads))
        {
            ads.PlayAds();
        }
    }
    public void PlayAds(EAdsType type,string placementId)
    {
        this.curIndex = (int)type;
        IAdsTool ads;
        if (this.m_dicAds.TryGetValue(type, out ads))
        {
            ads.PlayAds(placementId);
        }
    }
    public void PlayNext()
    {
        Debug.Log("PlayNext");
        if (curIndex >= 0)
        {
            Debug.Log("PlayNext1");
            nextCount++;
            if (nextCount >= Count)
            {
                if (this.playNextFailed != null)
                {
                    Debug.Log("PlayNext2");
                    this.playNextFailed();
                    this.nextCount = 0;
                    return;
                }
            }
            int index = (curIndex + 1) % this.Count;
            Debug.Log("PlayNext3:"+index);
            this.PlayAds((EAdsType)index);           
        }
    }
    public void SetPlayFinished(Action finished)
    {
        foreach (var ads in this.m_dicAds)
        {
            ads.Value.PlayFinished = finished;
        }
    }
    public void SetPlayFailed(Action<EAdsPlayFailedReason> failed)
    {
        foreach (var ads in this.m_dicAds)
        {
            ads.Value.PlayFailed = failed;
        }
    }
}
