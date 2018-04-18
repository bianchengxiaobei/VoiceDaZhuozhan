using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：StartState
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class StartState : ClientStateBase
{
    public override void OnEnter()
    {
        EventDispatch.Broadcast(Events.DlgStartShow);
        AudioManagerBase.Instance.PlayMusic("Assets.Audios.Musics.Main.mp3");
    }
    public override void OnLeave()
    {
        if (ClientGameStateManager.singleton.ENextGameState.Equals("TestState"))
        {
            return;
        }
        EventDispatch.Broadcast(Events.DlgStartHide);
        EventDispatch.Broadcast(Events.DlgLevelHide);
    }
}
