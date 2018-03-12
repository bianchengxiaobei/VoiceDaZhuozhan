using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
/*----------------------------------------------------------------
// 模块名：DlgLoading
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DlgLoading : UIBase
{
    public DlgLoading()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgLoading.prefab";
        this.mELayer = EUILayer.Top;
        this.mResident = true;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgLoadingShow, this.Show);
        EventDispatch.AddListener(Events.DlgLoadingHide, this.Hide);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        
    }

    public override void Realse()
    {
        
    }

    protected override void InitWidget()
    {
        
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener(Events.DlgLoadingAllFinished, this.OnLoadAllFinished);
    }

    protected override void OnRemoveListener()
    {
        //EventDispatch.RemoveListener(Events.DlgLoadingAllFinished, this.OnLoadAllFinished);
    }

    protected override void RealseWidget()
    {
        
    }
    public void OnLoadAllFinished()
    {
        PlayerManager.singleton.ShowMySelf();
        PlayerManager.singleton.ShowCurWaveMaster();
    }
}
