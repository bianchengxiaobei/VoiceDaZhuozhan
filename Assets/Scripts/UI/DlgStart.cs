using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
using UnityEngine.UI;
using DG.Tweening;
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
    public Button m_Button_Text;
    public Button m_Button_Setting;
    public Button m_Button_Ads;
    public Text m_Text_Money;
    private DOTweenAnimation LastAnimation;
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
        AdsManager.singleton.SetPlayFinished(this.OnAdsPlaySuccess);
        AdsManager.singleton.SetPlayFailed(this.OnAdsPlayFaield);
        AdsManager.singleton.playNextFailed = this.OnPlayNextFailed;
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        if (this.LastAnimation != null)
        {
            this.LastAnimation.onComplete.AddListener(() =>
            {
                if (!GuideModel.singleton.bIsGuideAllComp)
                {
                    GuideController.singleton.GuideFinishModelList(UserPrefsBase.singleton.guideFinishedId);
                }
            });
        }
        this.UpdateMoney();
    }

    public override void Realse()
    {
        
    }

    protected override void InitWidget()
    {
        this.m_Button_Start = this.mRoot.Find("bt_start").GetComponent<Button>();
        this.m_Button_Start.onClick.AddListener(this.OnClickStartButton);
        this.m_Button_Text = this.mRoot.Find("bt_test").GetComponent<Button>();
        this.m_Button_Text.onClick.AddListener(this.OnClickTextButton);
        this.m_Button_Ads = this.mRoot.Find("bt_ads").GetComponent<Button>();
        this.m_Button_Ads.onClick.AddListener(this.OnClickAdsButton);
        this.m_Button_Setting = this.mRoot.Find("bt_menu").GetComponent<Button>();
        this.m_Button_Setting.onClick.AddListener(this.OnClickSettingButton);
        this.LastAnimation = this.m_Button_Setting.GetComponent<DOTweenAnimation>();
        GuideController.singleton.AddGuideEventButton(this.m_Button_Ads.gameObject);
        GuideController.singleton.AddGuideEventButton(this.m_Button_Text.gameObject);
        this.m_Text_Money = this.mRoot.Find("sp_money/lb_money").GetComponent<Text>();
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener(Events.DlgStartMoneyUpdate, this.UpdateMoney);
    }

    protected override void OnRemoveListener()
    {
        EventDispatch.RemoveListener(Events.DlgStartMoneyUpdate, this.UpdateMoney);
    }

    protected override void RealseWidget()
    {
        
    }
    public void UpdateMoney()
    {
        this.m_Text_Money.text = UserPrefsBase.singleton.Money.ToString();
    }
    public void OnClickStartButton()
    {
        EventDispatch.Broadcast(Events.DlgLevelShow);
    }
    public void OnClickTextButton()
    {
        ClientGameStateManager.singleton.ChangeGameState("TestState");
    }
    public void OnClickAdsButton()
    {
        AdsManager.singleton.PlayAds(EAdsType.Vungle);
    }
    public void OnClickSettingButton()
    {
        //this.OnAdsPlaySuccess();
    }
    public void OnAdsPlaySuccess()
    {
        //发送给游戏服务器记录广告次数
        int time = -1;
        if (PlayerPrefs.HasKey(CommonDefineBase.Ads))
        {
            string[] content = PlayerPrefs.GetString(CommonDefineBase.Ads).Split(':');
            DateTime lastWatchTime = new DateTime(0);
            lastWatchTime.AddTicks(long.Parse(content[0]));
            time = int.Parse(content[1]);
            if (time == 0)
            {
                TimeSpan leftTime = DateTime.Now - lastWatchTime;
                if (leftTime.Hours >= 24)
                {
                    time = CommonDefineBase.AdsTime;
                }
                else
                {
                    EventDispatch.Broadcast(Events.DlgFlyTextShow);
                    EventDispatch.Broadcast<string>(Events.DlgAddSingleSystemInfo, "每天只能看5次广告哦!还要"+leftTime.Hours+"小时才能继续观看!");
                    return;
                }
            }
        }
        else
        {
            PlayerPrefs.SetString(CommonDefineBase.Ads, string.Format("{0}:{1}", DateTime.Now.Ticks, CommonDefineBase.AdsTime));
            time = CommonDefineBase.AdsTime;
        }
        time--;
        UserPrefsBase.singleton.AddMoney(200);
        PlayerPrefs.SetString(CommonDefineBase.Ads, string.Format("{0}:{1}", DateTime.Now.Ticks, time));
        Debug.Log("Success");
    }
    public void OnAdsPlayFaield(EAdsPlayFailedReason reason)
    {
        switch (reason)
        {
            case EAdsPlayFailedReason.NotReady:
                //Debug.Log("Play2323Next");
                AdsManager.singleton.PlayNext();
                break;
            case EAdsPlayFailedReason.NotWaitComplent:
                //提示玩家此次不会得到奖励
                //Debug.Log("22e");
                break;
            case EAdsPlayFailedReason.NotSupport:
                //Debug.Log("e2e2");
                AdsManager.singleton.PlayNext();
                break;
            case EAdsPlayFailedReason.Error:
                //Debug.Log("e2e1313122");
                AdsManager.singleton.PlayNext();
                break;
        }
    }
    public void OnPlayNextFailed()
    {
        Debug.Log("12312312PlayNext");
        EventDispatch.Broadcast(Events.DlgFlyTextShow);
        //显示玩家需要等广告缓冲一段时间
        EventDispatch.Broadcast<string>(Events.DlgAddSingleSystemInfo, "广告观看失败,请完整看完广告或者你的手机不支持广告功能！");
    }
}
