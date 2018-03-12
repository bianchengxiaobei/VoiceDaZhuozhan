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
    public void OnPointerDown(PointerEventData eventData)
    {
        VoiceManager.Instance.StartSpeech();
        EventDispatch.Broadcast<bool>(Events.DlgMainShowMask, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        VoiceManager.Instance.StopSpeech();
        EventDispatch.Broadcast<bool>(Events.DlgMainShowMask, false);
    }
}
