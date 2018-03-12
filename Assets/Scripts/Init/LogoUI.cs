using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：LogoUI
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LogoUI : MonoBehaviour
{
    public Image logo;
    public AudioSource audioSource;

    private static LogoUI m_oInstance;
    public static LogoUI Instance
    {
        get
        {
            if (m_oInstance != null)
            {
                return m_oInstance;
            }
            return null;
        }
    }

    private void Awake()
    {
        m_oInstance = this.transform.GetComponent<LogoUI>();
        if (!logo)
        {
            logo = this.transform.Find("sp_logo").GetComponent<Image>();
        }
        if (!audioSource)
        {
            audioSource = logo.transform.GetComponent<AudioSource>();
        }
    }
    public void PlayFadeStartLogo(Action finished)
    {
        Tweener tween = logo.DOFade(1, 5);
        tween.SetAutoKill();
        tween.SetEase(Ease.InOutExpo);
        TimerManager.AddTimer(3000, 0, () =>
        {
            audioSource.Play();
        });
        tween.OnComplete(() =>
        {
            if (finished != null)
            {
                finished();
            }
            this.Release();
        });
    }
    private void Release()
    {
        Destroy(this.gameObject);
        this.logo = null;
        this.audioSource = null;
        m_oInstance = null;
        //Resources.UnloadUnusedAssets();
    }
}
