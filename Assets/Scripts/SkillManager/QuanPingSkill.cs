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
    public override void Enter(EntityParent theOwner)
    {
        theOwner.PlayAnimation(skillConfig.animation);
        //延迟几秒，全屏怪
        if (this.skillConfig != null && this.skillConfig.param.Count > 0)
        {
            Param typeParam = this.skillConfig.param[0];
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
                        delay = (uint)(delayParam.param * 1000);
                        this.SlowDown(delay);
                        break;
                    case "控制":
                        delay = (uint)(delayParam.param * 1000);
                        break;
                }
                
            }
            //音频
            
        }
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
        });
    }
    private void SlowDown(uint delay)
    {
        TimerManager.AddTimer(delay, 0, () =>
        {
            SkillManager.singleton.SlowDown(this.skillConfig.skillId, true);
        });
    }
    private void Control(uint delay)
    {
        TimerManager.AddTimer(delay, 0, () =>
        {
            SkillManager.singleton.SlowDown(this.skillConfig.skillId, true);
        });
    }
}
