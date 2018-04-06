using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：SkillActor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class SkillActor
{
    public Skill skillConfig;
    public GameSkillBase skill;
    public EntityParent theOwner;
    public Transform theEntity;
    public Vector3 spawPos = Vector3.zero;
    protected bool m_bHasExited = false;
    public  virtual void Enter(Skill skill,EntityParent theOwner,GameSkillBase gameskill)
    {
        this.skillConfig = skill;
        this.theOwner = theOwner;
        this.skill = gameskill;
        this.CreateEntity();
    }
    public virtual void CreateEntity()
    {

    }
    public virtual void OnUpdate()
    {

    }
    public void SetSpawPos()
    {
        if (this.theEntity == null)
        {
            return;
        }
        switch (this.skillConfig.effectPos)
        {
            case EffectBindPos.右手:
                this.theEntity.SetParent(theOwner.bindNodeTable[CommonDefineBase.BindRightHand] as Transform);
                this.theEntity.localPosition = Vector3.zero;
                return;
            case EffectBindPos.中心:
                this.theEntity.SetParent(theOwner.bindNodeTable[CommonDefineBase.BindCenter] as Transform);
                this.theEntity.localPosition = Vector3.zero;
                return;
            case EffectBindPos.右脚:
                this.theEntity.SetParent(theOwner.bindNodeTable[CommonDefineBase.BindRightFoot] as Transform);
                this.theEntity.localPosition = Vector3.zero;
                return;
            case EffectBindPos.枪口:
                this.theEntity.SetParent(theOwner.bindNodeTable[CommonDefineBase.BindQiangkou] as Transform);
                this.theEntity.localPosition = Vector3.zero;
                return;
        }
        this.theEntity.SetParent(theOwner.Transform);
        this.theEntity.localPosition = Vector3.zero;
        theEntity.localRotation = Quaternion.identity;
        theEntity.localScale = Vector3.one;
    }
    public virtual void Exit()
    {
        this.m_bHasExited = true;
        this.skillConfig = null;
        this.theOwner = null;
    }
}
