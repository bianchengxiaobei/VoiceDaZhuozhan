using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：UIPlaySound
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class XUIPlaySound : MonoBehaviour
{
    public enum Trigger
    {
        OnClick,
        OnPress,
        OnSelect,
        Custom,
        OnEnable,
        OnDisable,
    }
    public AudioClip audioClip;
    public Trigger trigger = Trigger.OnClick;
    private Selectable eventObj;
    bool canPlay
    {
        get
        {
            if (!enabled) return false;
            return eventObj == null || eventObj.interactable;
        }
    }
    private void Awake()
    {
        eventObj = this.GetComponent<Selectable>();
        if (eventObj is Button)
        {
            (eventObj as Button).onClick.AddListener(this.OnClick);
        }
        if (eventObj is Toggle)
        {
            (eventObj as Toggle).onValueChanged.AddListener(this.OnSelect);
        }
    }
    void OnEnable()
    {
        if (trigger == Trigger.OnEnable)
            AudioManagerBase.Instance.PlayUIEffectSound(this.audioClip);
    }
    void OnDisable()
    {
        if (trigger == Trigger.OnDisable)
            AudioManagerBase.Instance.PlayUIEffectSound(this.audioClip);
    }
    private void OnClick()
    {
        if (canPlay && trigger == Trigger.OnClick)
        {
            AudioManagerBase.Instance.PlayUIEffectSound(this.audioClip);
        }
    }
    private void OnSelect(bool value)
    {
        if (canPlay && trigger == Trigger.OnSelect && value)
        {
            AudioManagerBase.Instance.PlayUIEffectSound(this.audioClip);
        }
    }
}
