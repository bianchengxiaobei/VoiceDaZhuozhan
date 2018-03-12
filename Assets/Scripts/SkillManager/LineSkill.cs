using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：LineSkill
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LineSkill : GameSkillBase
{
    public override void Enter(EntityParent theOwner)
    {
        if (actor == null)
        {
            actor = new LineSkillActor();
        }
        actor.Enter(skillConfig, theOwner);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void Exit()
    {
        
    }
}
