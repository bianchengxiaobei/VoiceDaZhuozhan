using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
/*----------------------------------------------------------------
// 模块名：StateForceMove
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class StateControll : IEntityState
{
    /// <summary>
    ///  进入该状态
    /// </summary>
    /// <param name="theOwner"></param>
    /// <param name="args"></param>
    public void Enter(EntityParent theOwner, params object[] args)
    {
        theOwner.CurrentMotionState = MotionState.Controll;
    }

    // 离开状态
    public void Exit(EntityParent theOwner, params object[] args)
    {
    }

    // 状态处理
    public void Process(EntityParent theOwner, params object[] args)
    {
        theOwner.SetSpeed(0);
        Vector3 oldDir = theOwner.entityActor.moveDir;
        theOwner.entityActor.moveDir = Vector3.zero;
        //创建眩晕特效
        uint time = (uint)((float)args[0] * 1000);
        TimerManager.AddTimer(time, 0, () => 
        {
            theOwner.entityActor.moveDir = oldDir;
            theOwner.ChangeMotionState(MotionState.WALKING);
        });
    }
    public void Execute(EntityParent theOwner)
    {
        
    }
}
