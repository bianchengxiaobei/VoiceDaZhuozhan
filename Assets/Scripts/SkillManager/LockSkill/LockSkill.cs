using System;
using System.Collections.Generic;
using CaomaoFramework;
public class LockSkill : GameSkillBase
{
    public List<EntityParent> lockedTargets = new List<EntityParent>();
    public uint EffectDelay = 0;
    private EntityParent m_theOwner;
    public override void Enter(EntityParent theOwner, bool test = false)
    {
        this.Test = test;
        m_theOwner = theOwner;
        theOwner.PlayAnimation(this.skillConfig.animation);
        Param delay = this.skillConfig.param[0];
        if (delay != null)
        {
            if (delay.param > 0)
            {
                EffectDelay = (uint)(delay.param * 1000);
            }
        }
        Param actorType = this.skillConfig.param[1];
        if ((int)(actorType.param) == (int)ActorType.Need)
        {
            if (actor == null)
            {
                actor = new LockSkillActor();
            }
            this.actor.Enter(this.skillConfig, theOwner, this);
        }
        else
        {
            this.m_theOwner.timer.Add(TimerManager.AddTimer(EffectDelay, 0, ()=> 
            {
                if (Test == false)
                {
                    UnityTool.CameraShake.Shake();
                }
                this.CaculateDamage();
                this.Exit();
            }));
            AudioType type = AudioType.飞鹰鸡西;
            switch (this.skillConfig.skillName)
            {
                case "飞鹰疾袭":
                    type = AudioType.飞鹰鸡西;
                    break;
            }
            //音频
            AudioManagerBase.Instance.LogicPlaySoundByClip(theOwner.audioSource,ResourcePoolManager.singleton.GetAudioClip(type));
        }
        base.Enter(theOwner, test);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public void CaculateDamage()
    {
        if (Test)
        {
            return;
        }
        Param typeParam = this.skillConfig.param[2];//技能类型
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
        if (this.skillConfig.param[3].name.Equals("锁定数量1级"))
        {
            int num = (int)this.skillConfig.param[3].param;
            float demage = this.skillConfig.demage[this.level-1].demage;
            List<EntityParent> monster = PlayerManager.singleton.GetNearestVisibleEntity(m_theOwner.Transform.position, num);
            foreach (var entity in monster)
            {
                entity.ApplyDemage(demage, this.skillConfig);
            }
        }
        else if(this.skillConfig.param[3].name.Equals("锁定数量(Random)"))
        {
            int num = (int)this.skillConfig.param[3].param;
            float demage = this.skillConfig.demage[this.level - 1].demage;
            List<EntityParent> monster = PlayerManager.singleton.GetRandomVisiableEntity(num);
            foreach (var entity in monster)
            {
                entity.ApplyDemage(demage, this.skillConfig);
            }
        }
    }
}
public enum ActorType
{
    NotNeed = 0,
    Need = 1
}
