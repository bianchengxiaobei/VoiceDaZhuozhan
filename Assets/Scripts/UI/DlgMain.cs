using System;
using CaomaoFramework;
using UnityEngine.UI;
using UnityEngine;
public class DlgMain : UIBase
{
    public GameObject SpeechMask;
    public Text text;
    public DlgMain()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgMain.prefab";
        this.mResident = false;
        this.mELayer = EUILayer.Middle;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgMainShow, this.Show);
        EventDispatch.AddListener(Events.DlgMainHide, this.Hide);
    }

    public override void OnDisable()
    {
       
    }

    public override void OnEnable()
    {
        this.ShowMask(false);
    }

    public override void Realse()
    {
       
    }

    protected override void InitWidget()
    {
        this.SpeechMask = this.mRoot.Find("sp_bg").gameObject;
        text = this.mRoot.Find("lb_test").GetComponent<Text>();  
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener<bool>(Events.DlgMainShowMask, this.ShowMask);
    }

    protected override void OnRemoveListener()
    {
        EventDispatch.RemoveListener<bool>(Events.DlgMainShowMask, this.ShowMask);
    }

    protected override void RealseWidget()
    {
       
    }
    public void ShowMask(bool value)
    {
        this.SpeechMask.SetActive(value);
    }
    public override void Update(float deltaTime)
    {
        this.text.text = VoiceManager.Instance.text;
    }
}

