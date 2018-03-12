using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：BattleController
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class BattleController : Singleton<BattleController>
{
    public void EnterGameMain()
    {
        //创建所有怪物的数据
        PlayerManager.singleton.CreateAllEntity();
        int levelId = LevelManager.singleton.currLevelId;
        SceneManager.singleton.CreateScene(levelId, new SceneBase());
        VoiceManager.Instance.RegisterCallback(PlayerManager.singleton.MySelf.CastSkill);
    }
}
