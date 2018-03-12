using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：GameMainState
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GameMainState : ClientStateBase
{
    public override void OnEnter()
    {
        PlayerManager.singleton.LoadAllEntity();
        SceneManager.singleton.LoadScene();
        EventDispatch.Broadcast(Events.DlgMainShow);
    }
    public override void OnLeave()
    {
        
    }
}
