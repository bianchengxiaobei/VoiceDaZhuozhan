using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
public class FSMMotion : FSMParent
{
    public FSMMotion()
    {
        m_theFSM.Add(MotionState.IDLE, new StateIdle());
        m_theFSM.Add(MotionState.WALKING, new StateWalking());
        m_theFSM.Add(MotionState.DEAD, new StateDead());
        m_theFSM.Add(MotionState.FORCEMOVE, new StateForceMove());
        m_theFSM.Add(MotionState.ReleaseSkill, new StateReleaseSkill());
    }
    public override void ChangeStatus(EntityParent owner, string newState, params object[] args)
    {
        if (owner.CurrentMotionState == newState && !newState.Equals(MotionState.FORCEMOVE))
        {
            //Debug.Log("重复状态：" + newState);
            return;
        }
        if (!m_theFSM.ContainsKey(newState))
        {
            return;
        }
        m_theFSM[owner.CurrentMotionState].Exit(owner, args);
        m_theFSM[newState].Enter(owner, args);
        m_theFSM[newState].Process(owner, args);
    }
    public override void Execute(EntityParent owner)
    {
        if (string.IsNullOrEmpty(owner.CurrentMotionState))
        {
            return;
        }
        if (!m_theFSM.ContainsKey(owner.CurrentMotionState))
        {
            return;
        }
        m_theFSM[owner.CurrentMotionState].Execute(owner);
    }
}
