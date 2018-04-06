using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class FixAreaSkill : GameSkillBase
{
    public uint EffectDelay = 0;
    public override void Enter(EntityParent theOwner,bool test = false)
    {
        this.Test = test;
        theOwner.PlayAnimation(skillConfig.skillName);
        if (this.skillConfig != null && this.skillConfig.param.Count > 0)
        {
            Param delayParam = this.skillConfig.param[1];
            if (delayParam != null)
            {
                if (delayParam.param > 0)
                {
                    EffectDelay = (uint)(delayParam.param * 1000);
                }
            }
        }
        if (this.actor == null)
        {
            this.actor = new FixAreaSkillActor();
        }
        this.actor.Enter(this.skillConfig,theOwner,this);
        base.Enter(theOwner);
    }
    public void CaculateDamage()
    {
        if (Test)
        {
            return;
        }
        Param typeParam = this.skillConfig.param[0];//技能类型
        if (typeParam != null)
        {
            switch (typeParam.name)
            {
                case "伤害":
                    this.Attack();
                    break;
                case "减速":
                    break;
                case "控制":
                    break;
            }
        }
    }
    public void Attack()
    {
        if (this.skillConfig.param[2].name.Equals("范围"))
        {
            float rangeParam = this.skillConfig.param[2].param;
            float demage = this.skillConfig.demage[0].demage;
            List<EntityParent> monster = PlayerManager.singleton.GetInRangeVisibleEntity(this.actor.theEntity.position, rangeParam);
            foreach (var entity in monster)
            {
                entity.ApplyDemage(demage,this.skillConfig);
            }
        }
    }
}

