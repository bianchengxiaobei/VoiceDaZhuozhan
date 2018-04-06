using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.Audio;
using System;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：DlgFlyText
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DlgFlyText : UIBase
{
    private Dictionary<EFlyTextType, IFlyTextManager> m_dicFlyTextManagers = new Dictionary<EFlyTextType, IFlyTextManager>();
    private XUIList List_Demage;
    private XUIList List_SysInfo;
    private XUIList List_Report;
    public DlgFlyText()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgFlyText.prefab";
        this.mResident = true;
        this.mELayer = EUILayer.SuperTop;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgFlyTextShow, this.Show);
        EventDispatch.AddListener(Events.DlgFlyTextHide, this.Hide);
    }

    public override void OnDisable()
    {

    }

    public override void OnEnable()
    {
        this.m_dicFlyTextManagers.Add(EFlyTextType.eFlyTextType_SystemInfo, new SystemInfoFlyTextManager(this.List_SysInfo));
        //this.m_dicFlyTextManagers.Add(EFlyTextType.eFlyTextType_Report, new ReportFlyTextManager(this.List_Report));
    }

    public override void Realse()
    {
        EventDispatch.RemoveListener(Events.DlgFlyTextShow, this.Show);
        EventDispatch.RemoveListener(Events.DlgFlyTextHide, this.Hide);
        //EventDispatch.RemoveListener<int, long, EHpEffectType>(Events.DlgAddHpEffect, this.AddHpEffect);
        //EventDispatch.RemoveListener<string>(Events.DlgAddReportInfo, this.AddReprotInfo);
        //EventDispatch.RemoveListener<long, long>(Events.DlgAddKillInfo, this.AddKillInfo);
        EventDispatch.RemoveListener<string>(Events.DlgAddSingleSystemInfo, this.AddSingleSystemInfo);
        EventDispatch.RemoveListener<string,Action,Action>(Events.DlgAddDoubleSystemInfo, this.AddDoubleSystemInfo);
        EventDispatch.RemoveListener<string, Action>(Events.DlgAddSingleActionSystemInfo, this.AddSingleActionSystemInfo);
    }

    protected override void InitWidget()
    {
        this.List_SysInfo = this.mRoot.Find("list_sysInfo").GetComponent<XUIList>();
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener<string>(Events.DlgAddSingleSystemInfo, this.AddSingleSystemInfo);
        EventDispatch.AddListener<string, Action, Action>(Events.DlgAddDoubleSystemInfo, this.AddDoubleSystemInfo);
        EventDispatch.AddListener<string, Action>(Events.DlgAddSingleActionSystemInfo, this.AddSingleActionSystemInfo);
    }

    protected override void OnRemoveListener()
    {
        
    }

    protected override void RealseWidget()
    {

    }
    public override void Update(float deltaTime)
    {
        foreach (var current in this.m_dicFlyTextManagers)
        {
            current.Value.Update();
        }
    }

    public void AddHpEffect(int hpCahnge, int heroId, EHpEffectType type)
    {
        if (hpCahnge != 0)
        {
            if (this.m_dicFlyTextManagers.ContainsKey(EFlyTextType.eFlyTextType_Hp))
            {
                string hpString = hpCahnge.ToString();
                if (hpCahnge > 0)
                {
                    hpString = string.Format("+{0}", hpString);
                }
                FlyTextEntity flyTextEntity = this.m_dicFlyTextManagers[EFlyTextType.eFlyTextType_Hp].Add(hpString, heroId);
                if (flyTextEntity != null)
                {
                    switch (type)
                    {
                        case EHpEffectType.eHpEffectType_Damage:
                            flyTextEntity.FlyTextItem.SetVisible("lb_hpGreen", false);//治疗效果的血量
                            flyTextEntity.FlyTextItem.SetVisible("lb_hpRed", true);
                            flyTextEntity.FlyTextItem.SetText("lb_hpRed", hpString);
                            break;
                        case EHpEffectType.eHpEffectType_Heal:
                            flyTextEntity.FlyTextItem.SetVisible("lb_hpGreen", true);
                            flyTextEntity.FlyTextItem.SetVisible("lb_hpRed", false);
                            flyTextEntity.FlyTextItem.SetText("lb_hpGreen", hpString);
                            break;
                    }
                }
            }
        }
    }
    public void AddSingleSystemInfo(string str)
    {
        if (this.m_dicFlyTextManagers.ContainsKey(EFlyTextType.eFlyTextType_SystemInfo))
        {
            FlyTextEntity flyTextEntity = this.m_dicFlyTextManagers[EFlyTextType.eFlyTextType_SystemInfo].Add(str, 0);
            if (flyTextEntity != null)
            {
                Button sinlebtn = flyTextEntity.FlyTextItem.GetButton("SingleSureButton");
                sinlebtn.onClick.RemoveAllListeners();
                sinlebtn.onClick.AddListener(() =>
                {
                    flyTextEntity.Active = false;
                });
                flyTextEntity.FlyTextItem.transform.Find("SingleSureButton").gameObject.SetActive(true);
                flyTextEntity.FlyTextItem.transform.Find("DoubleSureButton").gameObject.SetActive(false);
                flyTextEntity.FlyTextItem.transform.Find("DoubleCancelButton").gameObject.SetActive(false);
            }
        }
    }
    public void AddSingleActionSystemInfo(string str,Action action)
    {
        if (this.m_dicFlyTextManagers.ContainsKey(EFlyTextType.eFlyTextType_SystemInfo))
        {
            FlyTextEntity flyTextEntity = this.m_dicFlyTextManagers[EFlyTextType.eFlyTextType_SystemInfo].Add(str, 0);
            if (flyTextEntity != null)
            {
                Button sinlebtn = flyTextEntity.FlyTextItem.GetButton("SingleSureButton");
                sinlebtn.onClick.RemoveAllListeners();
                sinlebtn.onClick.AddListener(() =>
                {
                    flyTextEntity.Active = false;
                    if (action != null)
                    {
                        action();
                    }
                });
                flyTextEntity.FlyTextItem.transform.Find("SingleSureButton").gameObject.SetActive(true);
                flyTextEntity.FlyTextItem.transform.Find("DoubleSureButton").gameObject.SetActive(false);
                flyTextEntity.FlyTextItem.transform.Find("DoubleCancelButton").gameObject.SetActive(false);
            }
        }
    }
    public void AddDoubleSystemInfo(string str,Action sure,Action cancel)
    {
        if (this.m_dicFlyTextManagers.ContainsKey(EFlyTextType.eFlyTextType_SystemInfo))
        {
            FlyTextEntity flyTextEntity = this.m_dicFlyTextManagers[EFlyTextType.eFlyTextType_SystemInfo].Add(str, 0);
            if (flyTextEntity != null)
            {
                flyTextEntity.FlyTextItem.transform.Find("DoubleSureButton").gameObject.SetActive(true);
                flyTextEntity.FlyTextItem.transform.Find("DoubleCancelButton").gameObject.SetActive(true);
                flyTextEntity.FlyTextItem.transform.Find("SingleSureButton").gameObject.SetActive(false);
                Button doubleSureButton = flyTextEntity.FlyTextItem.GetButton("DoubleSureButton");
                doubleSureButton.onClick.RemoveAllListeners();
                doubleSureButton.onClick.AddListener(() =>
                {
                    flyTextEntity.Active = false;
                    if (sure != null)
                    {
                        sure();
                    }
                });
                Button doubleCancelButton = flyTextEntity.FlyTextItem.GetButton("DoubleCancelButton");
                doubleCancelButton.onClick.RemoveAllListeners();
                doubleCancelButton.onClick.AddListener(() =>
                {
                    flyTextEntity.Active = false;
                    if (cancel != null)
                    {
                        cancel();
                    }
                });
                flyTextEntity.FlyTextItem.transform.Find("SingleSureButton").gameObject.SetActive(false);
            }
        }
    }
    public void AddReprotInfo(string str)
    {
        if (this.m_dicFlyTextManagers.ContainsKey(EFlyTextType.eFlyTextType_Report))
        {
            this.m_dicFlyTextManagers[EFlyTextType.eFlyTextType_Report].Add(str, 0);
        }
    }
}