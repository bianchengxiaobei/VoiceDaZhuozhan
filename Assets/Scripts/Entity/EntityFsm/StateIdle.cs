using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
public class StateIdle : IEntityState
{
    /// <summary>
    ///  进入该状态
    /// </summary>
    /// <param name="theOwner"></param>
    /// <param name="args"></param>
    public void Enter(EntityParent theOwner, params object[] args)
    {
        theOwner.CurrentMotionState = MotionState.IDLE;
    }

    // 离开状态
    public void Exit(EntityParent theOwner, params object[] args)
    {
    }

    // 状态处理
    public void Process(EntityParent theOwner, params object[] args)
    {
        if (theOwner == null)
        {
            return;
        }
        theOwner.SetSpeed(0);
        theOwner.entityActor.moveDir = Vector3.zero;
    }
    public void Execute(EntityParent theOwner)
    {
        
    }
}
