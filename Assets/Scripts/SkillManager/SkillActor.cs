using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：SkillActor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class SkillActor
{
    public Skill skillConfig;
    public EntityParent theOwner;
    public Transform theEntity;
    public Vector3 spawPos = Vector3.zero;
    public void Enter(Skill skill,EntityParent theOwner)
    {
        this.skillConfig = skill;
        this.theOwner = theOwner;
        this.CreateEntity();
    }
    public virtual void CreateEntity()
    {

    }
    public virtual void OnUpdate()
    {

    }
}
