using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
using System;
/*----------------------------------------------------------------
// 模块名：StateDead
// 创建者：chen
// 修改者列表：
// 创建日期：2017.7.9
// 模块描述：死亡状态
//--------------------------------------------------------------*/
/// <summary>
/// 死亡状态
/// </summary>
public class StateDead : IEntityState
{
    public void Enter(EntityParent entity, params object[] args)
    {
        entity.CurrentMotionState = MotionState.DEAD;
    }

    public void Exit(EntityParent entity, params object[] args)
    {
        
    }
    public void Process(EntityParent entity, params object[] args)
    {
        entity.animator.CrossFade("Dead", 0.0f);
        entity.bIsDead = true;
        //播放死亡音效
    }
    public void Execute(EntityParent theOwner)
    {

    }
}
