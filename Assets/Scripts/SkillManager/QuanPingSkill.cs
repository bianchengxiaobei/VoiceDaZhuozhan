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
            Param param = this.skillConfig.param[0];
            if (param != null)
            {
                uint delay = (uint)(param.param * 1000);
                TimerManager.AddTimer(delay, 0, () => 
                {
                    SkillManager.singleton.Attack(this.skillConfig.skillId, true);
                });
            }
        }
    }
    public override void OnUpdate()
    {
        
    }
    public override void Exit()
    {
        base.Exit();
    }
}
