using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：DlgStart
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DlgStart : UIBase
{
    public Button m_Button_Start;
    public DlgStart()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgStart.prefab";
        this.mELayer = EUILayer.Bottom;
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgStartShow, this.Show);
        EventDispatch.AddListener(Events.DlgStartHide, this.Hide);
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
        this.m_Button_Start = this.mRoot.Find("bt_start").GetComponent<Button>();
        this.m_Button_Start.onClick.AddListener(this.OnClickStartButton);
    }

    protected override void OnAddListener()
    {
        
    }

    protected override void OnRemoveListener()
    {
        
    }

    protected override void RealseWidget()
    {
        
    }
    public void OnClickStartButton()
    {
        EventDispatch.Broadcast(Events.DlgLevelShow);
    }
}
