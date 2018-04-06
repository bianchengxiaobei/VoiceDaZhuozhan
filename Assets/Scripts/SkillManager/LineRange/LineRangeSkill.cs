using System;
using System.Collections.Generic;

public class LineRangeSkill : GameSkillBase
{
    public uint EffectDelay = 0;
    public override void Enter(EntityParent theOwner,bool test = false)
    {
        this.Test = test;
        theOwner.PlayAnimation(skillConfig.animation);
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
            this.actor = new LineRangeSkillActor();
        }
        this.actor.Enter(this.skillConfig, theOwner, this);
        base.Enter(theOwner);
    }
    public void Attack(EntityParent entitys)
    {
        float demage = this.skillConfig.demage[this.level - 1].demage;
        entitys.ApplyDemage(demage,this.skillConfig);
    }
}

