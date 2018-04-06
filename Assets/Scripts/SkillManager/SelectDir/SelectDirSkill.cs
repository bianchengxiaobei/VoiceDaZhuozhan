using System;
using System.Collections.Generic;

public class SelectDirSkill : GameSkillBase
{
    public string firstAnim;
    public string secondAnim;
    public string thirdAnim;
    public uint EffectDelay = 0;
    public override void Enter(EntityParent theOwner, bool test = false)
    {
        this.Test = test;
        this.firstAnim = this.skillConfig.param[0].name;
        this.secondAnim = this.skillConfig.param[1].name;
        this.thirdAnim = this.skillConfig.param[2].name;
        if (this.skillConfig != null && this.skillConfig.param.Count > 0)
        {
            Param delayParam = this.skillConfig.param[3];
            if (delayParam != null)
            {
                if (delayParam.param > 0)
                {
                    EffectDelay = (uint)(delayParam.param * 1000);
                }
            }
        }
        theOwner.PlayAnimation(this.firstAnim);
        if (this.actor == null)
        {
            actor = new SelectDirSkillActor();
            actor.Enter(this.skillConfig, theOwner, this);
        }
        base.Enter(theOwner, test);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public void CaculateDamage(EntityParent monster)
    {
        if (monster.bIsDead)
        {
            return;
        }
        float demage = this.skillConfig.demage[this.level - 1].demage;
        monster.ApplyDemage(demage,this.skillConfig);
    }
}

