using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillView : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public Text cd;
    public GameObject mask;
    public Image skillIcon;
    private int skillId = int.MinValue;
    void Start ()
    {
        this.mask.SetActive(false);
        this.cd.gameObject.SetActive(false);
	}
	
	void Update ()
    {
		
	}
    public void SetSkillNameAndId(int skillId)
    {
        this.skillId = skillId;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        GameSkillBase skill = SkillManager.singleton.GetSkill(this.skillId);
        PlayerManager.singleton.MySelf.CastSkill(skill.skillConfig.skillToken);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
