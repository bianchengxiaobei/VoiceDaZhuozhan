﻿using System.Collections;
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
        m_theFSM.Add(MotionState.Controll, new StateControll());
        m_theFSM.Add(MotionState.ReleaseSkill, new StateReleaseSkill());
        m_theFSM.Add(MotionState.HIT, new StateOnHit());
        m_theFSM.Add(MotionState.Attack, new StateAttack());
    }
    public override void ChangeStatus(EntityParent owner, string newState, params object[] args)
    {
        if (owner.CurrentMotionState == newState)
        {
            //Debug.Log("重复状态：" + newState);
            if (newState.Equals(MotionState.Controll) || newState.Equals(MotionState.ReleaseSkill))
            {
                goto Process;
            }
            return;
        }
        if (!m_theFSM.ContainsKey(newState))
        {
            return;
        }
        m_theFSM[owner.CurrentMotionState].Exit(owner, args);
        m_theFSM[newState].Enter(owner, args);
        Process:
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
