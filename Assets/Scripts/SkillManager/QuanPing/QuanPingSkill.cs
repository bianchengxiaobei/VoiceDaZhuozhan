using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：QuanPingSkill
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class QuanPingSkill : GameSkillBase
{
    public override void Enter(EntityParent theOwner,bool test = false)
    {
        this.Test = test;
        theOwner.PlayAnimation(skillConfig.animation);
        //延迟几秒，全屏怪
        if (this.skillConfig != null && this.skillConfig.param.Count > 0 && test == false)
        {
            Param typeParam = this.skillConfig.param[0];//技能类型
            Param delayParam = this.skillConfig.param[1];
            if (typeParam != null)
            {
                uint delay = 0;
                switch (typeParam.name)
                {
                    case "伤害":
                        delay = (uint)(delayParam.param * 1000);
                        this.Attack(delay);
                        break;
                    case "减速":
                        Param durationParam = this.skillConfig.param[2];
                        delay = (uint)(delayParam.param * 1000);
                        this.SlowDown(delay,durationParam.param);
                        break;
                    case "控制":
                        delay = (uint)(delayParam.param * 1000);
                        this.Control(delay);
                        break;
                }

            }
            base.Enter(theOwner, test);
        }
        //音频
        AudioType type = AudioType.召唤圣高;
        switch (this.skillConfig.skillName)
        {
            case "召唤诸葛琴魔":
                type = AudioType.召唤诸葛;
                break;
            case "召唤圣主高飞":
                type = AudioType.召唤圣高;
                break;
            case "龙之斩":
                type = AudioType.龙之斩;
                break;
        }
        AudioManagerBase.Instance.LogicPlaySoundByClip(theOwner.audioSource, ResourcePoolManager.singleton.GetAudioClip(type));
    }
    public override void OnUpdate()
    {

    }
    public override void Exit()
    {
        base.Exit();
    }
    private void Attack(uint delay)
    {
        TimerManager.AddTimer(delay, 0, () =>
        {
            SkillManager.singleton.Attack(this.skillConfig.skillId, true);
            this.Exit();
        });
    }
    private void SlowDown(uint delay,float duration)
    {
        TimerManager.AddTimer(delay, 0, () =>
        {
            SkillManager.singleton.SlowDown(this.skillConfig.skillId, true,duration);
            this.Exit();
        });
    }
    private void Control(uint delay)
    {
        TimerManager.AddTimer(delay, 0, () =>
        {
            SkillManager.singleton.Control(this.skillConfig.skillId, true);
            this.Exit();
        });
    }
}
