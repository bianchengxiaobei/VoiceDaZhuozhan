using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CaomaoFramework;
using DG.Tweening;
public class SkillView : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public Text m_cdNumText;
    public Image mask;
    public Image skillIcon;
    private int skillId = int.MinValue;
    public float m_fCdNum;
    private float timerSum;
    private Tweener tween;
    void Start ()
    {
        this.mask.gameObject.SetActive(false);
        this.m_cdNumText.gameObject.SetActive(false);
	}
	
	void Update ()
    {
        if (this.m_fCdNum > 0)
        {
            if (this.m_cdNumText && !this.m_cdNumText.gameObject.activeSelf)
            {
                this.m_cdNumText.gameObject.SetActive(true);
            }
            this.m_fCdNum -= Time.deltaTime;
            this.timerSum += Time.deltaTime;
            if (this.m_fCdNum > 1f)
            {
                if (this.timerSum >= 0.5f)
                {
                    this.timerSum = 0f;
                    this.TextTimer();
                }
            }
            else if (this.timerSum >= 0.1f)
            {
                this.timerSum = 0f;
                this.TextTimer();
            }
        }
        else if (this.m_cdNumText.gameObject.activeSelf)
        {
            this.TextTimer();
            this.m_cdNumText.gameObject.SetActive(false);
        }
    }
    public void SetSkillNameAndId(int skillId)
    {
        this.skillId = skillId;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!UnityMonoDriver.s_instance.ReleaseMode)
        {
            GameSkillBase skill = SkillManager.singleton.GetSkill(this.skillId);
            PlayerManager.singleton.MySelf.CastSkill(skill.skillConfig.skillToken);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void SetActiveMask(bool active)
    {
        if (this.mask != null)
        {
            this.mask.gameObject.SetActive(active);
        }
    }
    public void SetFillAmount(float value)
    {
        if (this.mask != null)
        {
            this.mask.fillAmount = value;
        }
    }
    public void StartTweenCD(float duration, float endValue = 0f)
    {
        if (this.mask != null)
        {
            this.tween = this.mask.DOFillAmount(endValue, duration).OnComplete(() =>
            {
                this.mask.gameObject.SetActive(false);
            });
        }
    }
    public void EndCD()
    {
        this.m_fCdNum = 0;
        this.timerSum = 0;
        this.SetActiveMask(false);
        if (this.tween != null)
        {
            this.tween.Kill();
        }
    }
    public void StartCDTextNumTimer(float duration)
    {
        this.m_fCdNum = duration;
    }
    private void TextTimer()
    {
        if (this.m_fCdNum > 0f)
        {
            if (this.m_fCdNum < 1f)
            {
                this.m_cdNumText.text = this.m_fCdNum.ToString("0.0");
            }
            else
            {
                this.m_cdNumText.text = this.m_fCdNum.ToString("0");
            }
            return;
        }
        if (this.m_cdNumText)
        {
            this.m_cdNumText.text = string.Empty;
        }
    }
}
