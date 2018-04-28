using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：GuideForceClick
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideForceClick : GuideTaskBase
{
    private GameObject EventButton;
    private GameObject ForceEffect;
    private GameObject ForceClickPrefab;
    private GameObject Black;//黑色遮罩
    private Transform EventButtonTempParent;
    private DataGuideTaskInfo data;
    private GameObject ShowView;
    private VoiceController controller;
    public GuideForceClick(int taskId, EGuideTaskType type, GameObject parent) : base(taskId, type, parent)
    {

    }
    public override void EnterTask()
    {
        data = GameData<DataGuideTaskInfo>.dataMap[this.TaskId];
        EventDispatch.AddListener<GameObject>(Events.DlgAddGuideEvent, this.OnUIOpenAddButtonEvent);
        //添加事件
        this.ShowGuide();
    }
    private void ShowGuide()
    {      
        if (data == null)
        {
            this.FinishTask();
            return ;
        }
        string btnName = data.BtnName;
        if (!string.IsNullOrEmpty(btnName))
        {
            this.EventButton = GuideModel.singleton.GetUIGuideButtonGameObject(btnName);
            if (this.EventButton == null)
            {
                Debug.Log("不存在EventButton:"+btnName);
                if (GuideModel.singleton.UIGuideButtonGameObject.ContainsKey(btnName))
                {
                    GuideModel.singleton.UIGuideButtonGameObject.Remove(btnName);
                }
                return;
            }
            this.LoadForceEffect();
            this.LoadTextInfo();
            this.ShowAndAddButtonEvent();
        }
        else
        {
            Debug.LogError("Task.BtnName == null");
        }
    }
    /// <summary>
    /// 加载UI特效
    /// </summary>
    private void LoadForceEffect()
    {
        if (data != null && this.EventButton != null)
        {
            if (string.IsNullOrEmpty(data.GuideEffectPath))
            {
                return;
            }
            WWWResourceManager.Instance.Load(data.GuideEffectPath, (asset) => 
            {
                if (asset != null)
                {
                    this.ForceEffect = asset.Instantiate();
                    this.ForceEffect.transform.SetParent(EventButton.transform);
                    this.ForceEffect.transform.localPosition = Vector3.back;
                    this.ForceEffect.transform.localRotation = Quaternion.identity;
                    this.ForceEffect.transform.localScale = Vector3.one * 100;
                }
            });
        }
    }
    /// <summary>
    /// 加载向导信息
    /// </summary>
    private void LoadTextInfo()
    {
        if (data != null && this.EventButton != null)
        {
            if (string.IsNullOrEmpty(data.UIInfoFrame))
            {
                return;
            }
            WWWResourceManager.Instance.Load(data.UIInfoFrame, (asset) =>
            {
                if (asset != null)
                {
                    this.ForceClickPrefab = asset.Instantiate();
                    this.ForceClickPrefab.transform.SetParent(this.mRoot.transform);
                    Transform Info = this.ForceClickPrefab.transform.Find("lb_info");
                    if (Info != null)
                    {
                        Text text = Info.GetComponent<Text>();
                        if (text != null)
                        {
                            if (string.IsNullOrEmpty(data.GuideText))
                            {
                                Debug.Log("GuideInfo == null");
                            }
                            text.text = data.GuideText;
                        }                       
                    }     
                    this.ForceClickPrefab.transform.localPosition = data.InfoPos;
                    this.ForceClickPrefab.transform.localScale = Vector3.one;
                }
            });
        }
    }
    private void ShowAndAddButtonEvent()
    {
        this.Black = this.mRoot.transform.Find("sp_mask").gameObject;
        this.EventButtonTempParent = EventButton.transform.parent;
        EventButton.transform.SetParent(this.Black.transform);
        if (this.data.BtnPos != null && this.data.BtnPos != Vector3.zero)
        {
            EventButton.transform.localPosition = this.data.BtnPos;
            EventButton.transform.localScale = Vector3.one;
        }
        if (!string.IsNullOrEmpty(this.data.ShowView))
        {
            this.ShowView = this.EventButtonTempParent.Find(this.data.ShowView).gameObject;
            if (this.ShowView != null)
            {
                this.ShowView.transform.SetParent(this.Black.transform);
                if (this.data.ShowViewPos != null && this.data.ShowViewPos != Vector3.zero)
                {
                    this.ShowView.transform.localPosition = this.data.ShowViewPos;
                    this.ShowView.transform.localScale = Vector3.one;
                }
            }
        }
        if (!this.Black.activeSelf)
        {
            this.Black.SetActive(true);
        }
        if (data != null)
        {
            if (data.BtnTriggerType == (int)EButtonTriggerType.Click)
            {
                Selectable click = null;
                if (EventButton != null)
                {
                    click = EventButton.GetComponent<Button>();
                    if (click != null)
                    {
                        (click as Button).onClick.AddListener(this.OnClick);
                    }
                    else
                    {
                        click = EventButton.GetComponent<Toggle>();
                        if (click != null)
                        {
                            (click as Toggle).onValueChanged.AddListener(this.OnSelect);
                        }
                        else
                        {
                            controller = EventButton.GetComponent<VoiceController>();
                            if (controller != null)
                            {
                                controller.onClickDown += this.OnClickDown;
                                controller.onClickUp += this.OnClickUp;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnClick()
    {
        this.FinishTask();
    }
    public void OnClickDown()
    {
        this.bTaskCoolDown = true;
        this.m_fTaskTime = Time.realtimeSinceStartup;
        this.m_fTaskCDtime = 1;
    }
    public void OnClickUp()
    {
        this.bTaskCoolDown = false;
    }
    private void OnSelect(bool value)
    {
       this.FinishTask();
    }
    private void OnDrag()
    {
       this.FinishTask();
    }
    public override void FinishTask()
    {
        this.EventButton.transform.SetParent(this.EventButtonTempParent);
        if (this.ShowView != null)
        {
            this.ShowView.transform.SetParent(this.EventButtonTempParent);
        }
        this.EventButtonTempParent = null;

        if (data.BtnTriggerType == (int)EButtonTriggerType.Click)
        {
            Selectable eventBtn = this.EventButton.GetComponent<Selectable>();
            if (eventBtn != null)
            {
                if (eventBtn is Button)
                {
                    (eventBtn as Button).onClick.RemoveListener(this.OnClick);
                }
                else
                {
                    (eventBtn as Toggle).onValueChanged.RemoveListener(this.OnSelect);
                }
            }
            else
            {
                if (controller != null)
                {
                    controller.onClickDown -= this.OnClick;
                    controller.onClickUp -= this.OnClickUp;
                    controller.onClickDown = null;
                    controller.onClickUp = null;
#if UNITY_ANDROID
                    VoiceManager.Instance.StartSpeech();
#elif UNITY_IOS
                    VoiceManager.Instance.StartSpeech_IOS();
#endif
                    EventDispatch.Broadcast<bool>(Events.DlgTextShowMask, true);
                }
            }
        }
        this.controller = null;
        this.data = null;
        this.ShowView = null;
        base.FinishTask();
    }
    public override void ClearTask()
    {
        if (ForceEffect != null)
        {
            GameObject.Destroy(ForceEffect);
        }
        if (this.ForceClickPrefab != null)
        {
            GameObject.Destroy(this.ForceClickPrefab);
        }
        this.Black.SetActive(false);
        EventDispatch.RemoveListener<GameObject>(Events.DlgAddGuideEvent,this.OnUIOpenAddButtonEvent);
    }
    private void OnUIOpenAddButtonEvent(GameObject obj)
    {
        if (obj.name == data.BtnName)
        {
            ShowGuide();
        }
    }
    public override void ExcuseTask()
    {
        if (!bTaskCoolDown)
        {
            return;
        }
        if (Time.realtimeSinceStartup - m_fTaskTime >= m_fTaskCDtime)
        {
            bTaskCoolDown = false;
            this.FinishTask();
        }
    }
}
public enum EButtonTriggerType
{
    Click,
    Drag
}
