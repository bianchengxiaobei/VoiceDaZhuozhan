using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class StateAttack : IEntityState
{
    public uint AttackTimer;
    public void Enter(EntityParent entity, params object[] args)
    {
        entity.CurrentMotionState = MotionState.Attack;
    }

    public void Execute(EntityParent entity)
    {
        if (PlayerManager.singleton.MySelf == null || PlayerManager.singleton.MySelf.bIsDead)
        {
            entity.Idle();
        }
    }

    public void Exit(EntityParent entity, params object[] args)
    {
        TimerManager.DelTimer(this.AttackTimer);
    }

    public void Process(EntityParent entity, params object[] args)
    {
        entity.SetSpeed(0);
        this.AttackTimer = TimerManager.AddTimer(0,2000,()=> 
        {
            entity.PlayAnimation("Attack");
            PlayerManager.singleton.MySelf.ApplyDemage(entity.demage,null);
        });
    }
}
