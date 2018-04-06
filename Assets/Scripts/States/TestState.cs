using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class TestState : ClientStateBase
{
    public override void OnEnter()
    {
        EventDispatch.Broadcast(Events.DlgTextShow);
    }
    public override void OnLeave()
    {
        EventDispatch.Broadcast(Events.DlgTextHide);
    }
}
