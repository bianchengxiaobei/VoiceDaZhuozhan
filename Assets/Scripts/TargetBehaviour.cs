using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TargetBehaviour : MonoBehaviour
{
    public TextMesh text;
    public SelectDirSkill skill;
    private int num = 1;
    public void OnMouseDown()
    {
        this.text.gameObject.SetActive(false);
    }
    private void OnMouseDrag()
    {
        Vector3 pos = Vector3.zero;
        if (skill.Test)
        {
            pos = UnityTool.UICamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }       
        pos.z = -1;
        transform.position = pos;
    }
    private void OnMouseUpAsButton()
    {
        if (num <= (int)skill.skillConfig.param[skill.level + 3].param)
        {
            ((SelectDirSkillActor)skill.actor).OpenFire();
        }
        else
        {
            ((SelectDirSkillActor)skill.actor).EndFire();
            return;
        }
        num++;
        this.OnMouseUpAsButton();
    }
    public void Clear()
    {
       num = 1;
       skill = null;
    }
}
