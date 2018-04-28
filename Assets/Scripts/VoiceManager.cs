using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
/*----------------------------------------------------------------
// 模块名：VoiceManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;


    public Action<string> speedEndCallback;
    public Action<string> speedEndError;
    private void Awake()
    {
        Instance = this;
    }
#if UNITY_ANDROID
    AndroidJavaObject jo;
    private string appId = "5a65ea92";
    private void Start()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        if (string.IsNullOrEmpty(appId))
        {
            Debug.LogError("AppId == null");
            return;
        }
        jo.Call("CreateAppId", appId);
        this.StartSpeech();
    }
    public void StartSpeech()
    {
        if (jo != null)
        {
            jo.Call("StartSpeech");
        }
    }
    public void StopSpeech()
    {
        if (jo != null)
        {
            jo.Call("StopSpeech");
        }
    }
    public void CancelSpeech()
    {
        if (jo != null)
        {
            jo.Call("CancelSpeech");
        }
    }
#elif UNITY_IOS
    [DllImport("__Internal")]
    public static extern void _StartSpeech();
    [DllImport("__Internal")]
    public static extern void _CreateAppId(string appId);
    [DllImport("__Internal")]
    public static extern void _StopSpeech();
    [DllImport("__Internal")]
    public static extern void _CancelSpeech();

    private string appId = "5acc46b3";
    private void Start()
    {
        if (string.IsNullOrEmpty(appId))
        {
            Debug.LogError("AppId == null");
            return;
        }
        _CreateAppId(appId);
        _StartSpeech();
    }
    public void StartSpeech_IOS()
    {
        _StartSpeech();
    }
    public void StopSpeech_IOS()
    {
        _StopSpeech();
    }
    public void CancelSpeech_IOS()
    {
        _CancelSpeech();
    }
#endif
    public void OnInit(string errorCode)
    {
        Debug.Log("OnInit:" + errorCode);
    }
    public void OnStartListening(string errorCode)
    {
        Debug.Log("OnStartListening:" + errorCode);
    }
    public void OnBeginSpeech(string value)
    {
        Debug.Log("OnBeginSpeech");
    }
    public void OnEndSpeech(string end)
    {
        Debug.Log("OnEndSpeech");
    }
    public void OnSpeechError(string error)
    {
        Debug.LogError("Error:" + error);
        if (this.speedEndError != null)
        {
            this.speedEndError(error);
        }
    }
    public void OnEvent(string args)
    {
        Debug.Log(args);
    }
    public void OnResult(string speechContent)
    {
        if (this.speedEndCallback != null)
        {
            this.speedEndCallback(speechContent);
        }
    }
    public void TestResult(string content)
    {
        Debug.Log(content);
    }
    public void OnVolumeChanged(string args)
    {
        Debug.Log("valueChange:" + args);
    }
    public void RegisterCallback(Action<string> call,Action<string> error = null)
    {
        if (this.speedEndCallback != null)
        {
            this.speedEndCallback = null;
        }
        if (this.speedEndCallback == null)
        {
            this.speedEndCallback = call;
        }
        if (error != null)
        {
            this.speedEndError = error;
        }
    }
    public void UnRegisterCallback()
    {
        if (this.speedEndCallback != null)
        {
            this.speedEndCallback = null;
        }
        if (this.speedEndError != null)
        {
            this.speedEndError = null;
        }
    }
}
