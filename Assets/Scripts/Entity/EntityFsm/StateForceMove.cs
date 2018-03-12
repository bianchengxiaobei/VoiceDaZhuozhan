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
public class StateForceMove : IEntityState
{
    /// <summary>
    ///  进入该状态
    /// </summary>
    /// <param name="theOwner"></param>
    /// <param name="args"></param>
    public void Enter(EntityParent theOwner, params object[] args)
    {
        theOwner.CurrentMotionState = MotionState.FORCEMOVE;
    }

    // 离开状态
    public void Exit(EntityParent theOwner, params object[] args)
    {
    }

    // 状态处理
    public void Process(EntityParent theOwner, params object[] args)
    {
        
    }
    public void Execute(EntityParent theOwner)
    {
        
    }
}
