using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using CaomaoFramework;
using System;
/*----------------------------------------------------------------
// 模块名：VoiceController
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class VoiceController : MonoBehaviour, IPointerUpHandler,IPointerDownHandler
{
    public Action onClickDown;
    public Action onClickUp;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onClickDown != null)
        {
            onClickDown();
            return;
        }
        VoiceManager.Instance.StartSpeech();
        EventDispatch.Broadcast<bool>(Events.DlgMainShowMask, true);
        EventDispatch.Broadcast<bool>(Events.DlgTextShowMask, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onClickUp != null)
        {
            onClickUp();
            return;
        }
        VoiceManager.Instance.StopSpeech();
        EventDispatch.Broadcast<bool>(Events.DlgMainShowMask, false);
        EventDispatch.Broadcast<bool>(Events.DlgTextShowMask, false);
    }
}
