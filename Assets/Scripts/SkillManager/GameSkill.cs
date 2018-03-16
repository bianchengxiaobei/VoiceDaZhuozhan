using UnityEngine;
using System.Collections.Generic;
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
    public bool bLocked;
    public Skill skillConfig;
    public int level;
    public SkillActor actor;

    public void Init(Skill skill)
    {
        this.skillConfig = skill;
    }
    public virtual void Enter(EntityParent theOwner)
    {

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
        if (PlayerManager.singleton.MySelf != null)
        {
            PlayerManager.singleton.MySelf.Idle();
        }
    }
}
