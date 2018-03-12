using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
public class StateWalking : IEntityState
{
    public void Enter(EntityParent theOwner, params object[] args)
    {
        theOwner.CurrentMotionState = MotionState.WALKING;
    }
    public void Exit(EntityParent theOwner, params object[] args)
    {
        
    }
    public void Process(EntityParent theOwner, params object[] args)
    {
        theOwner.SetSpeed(1);
    }
    public void Execute(EntityParent theOwner)
    {
        
    }
}
