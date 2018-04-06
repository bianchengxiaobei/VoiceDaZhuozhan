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
    private int curSkillId;
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
        this.curSkillId = 0;
    }

    // 状态处理
    public void Process(EntityParent theOwner, params object[] args)
    {
        skill = (GameSkillBase)args[0];
        if (SkillManager.singleton.IsSkillInRunning(skill.skillConfig.skillId))
        {
            Debug.Log("runing:" + skill.skillConfig.skillName);
            return;
        }
        if (skill != null && skill.skillConfig != null)
        {
            skill.Enter(theOwner);
            this.curSkillId = skill.skillConfig.skillId;
        }
    }
    public void Execute(EntityParent theOwner)
    {
        
    }
}
