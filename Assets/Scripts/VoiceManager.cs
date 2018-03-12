using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
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
    public string appId;

    AndroidJavaObject jo;
    public string text;
    public Action<string> speedEndCallback;
    private void Awake()
    {
        Instance = this;
    }
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
    }
    public void OnEvent(string args)
    {
        Debug.Log(args);
    }
    public void OnResult(string speechContent)
    {
        text = speechContent;
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
    public void RegisterCallback(Action<string> call)
    {
        if (this.speedEndCallback == null)
        {
            this.speedEndCallback = call;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
