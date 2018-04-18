using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：GameSkill
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GameSkillBase
{
    public bool bLocked = false;
    public Skill skillConfig;
    public int level = 1;
    public SkillActor actor;
    public bool Test = false;
    public void Init(Skill skill)
    {
        this.skillConfig = skill;
    }
    public virtual void Enter(EntityParent theOwner,bool test = false)
    {
        SkillManager.singleton.runningSkills.Add(this);
    }
    public virtual void OnUpdate()
    {
        if (actor != null)
        {
            actor.OnUpdate();
        }
    }
    public virtual void Exit()
    {
        EventDispatch.Broadcast<int>(Events.DlgMainStartCd, this.skillConfig.skillId);
        if (PlayerManager.singleton.MySelf != null)
        {
            PlayerManager.singleton.MySelf.Idle();
        }
        if (actor != null)
        {
            actor.Exit();
            actor = null;
        }
        SkillManager.singleton.runningSkills.Remove(this);
    }
}
