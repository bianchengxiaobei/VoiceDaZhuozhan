using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
/*----------------------------------------------------------------
// 模块名：StateReleaseSkill
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class StateReleaseSkill : IEntityState
{
    GameSkillBase skill;
    /// <summary>
    ///  进入该状态
    /// </summary>
    /// <param name="theOwner"></param>
    /// <param name="args"></param>
    public void Enter(EntityParent theOwner, params object[] args)
    {
        theOwner.CurrentMotionState = MotionState.ReleaseSkill;
    }

    // 离开状态
    public void Exit(EntityParent theOwner, params object[] args)
    {
        skill = null;
    }

    // 状态处理
    public void Process(EntityParent theOwner, params object[] args)
    {
        skill = (GameSkillBase)args[0];
        if (skill != null && skill.skillConfig != null)
        {
            skill.Enter(theOwner);
        }
    }
    public void Execute(EntityParent theOwner)
    {
        if (skill != null)
        {
            skill.OnUpdate();
        }
    }
}
