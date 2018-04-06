using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：GuideModel
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideModel : Singleton<GuideModel>
{
    public int NowTaskId;
    public bool bIsGuideAllComp;
    public bool bIsGuideBattle = false;
    public bool bIsFirstLocked = false;
    public bool bIsSelfSpeaked = false;
    public DataGuideParentTaskInfo TaskMrgData;
    public EGuideStepInfo CurrentTaskModelId;//现在的任务模块Id
    /// <summary>
    /// key->Name V:GameObject
    /// </summary>
    public Dictionary<string, GameObject> UIGuideButtonGameObject = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> UIGuideSpriteGameObject = new Dictionary<string, GameObject>();
    public List<GuideTaskBase> GuideTaskExecuteList = new List<GuideTaskBase>();
    /// <summary>
    /// 已经完成的向导模块
    /// </summary>
    public List<int> GuideFinishedList = new List<int>();
    
    /// <summary>
    /// 取得Guide缓存的Button物体
    /// </summary>
    /// <param name="btnName"></param>
    /// <returns></returns>
    public GameObject GetUIGuideButtonGameObject(string btnName)
    {
        if (UIGuideButtonGameObject.ContainsKey(btnName))
        {
            return this.UIGuideButtonGameObject[btnName];
        }
        return null;
    }
    public GameObject GetUIGuideSpriteGameObject(string spriteName)
    {
        if (UIGuideSpriteGameObject.ContainsKey(spriteName))
        {
            return this.UIGuideSpriteGameObject[spriteName];
        }
        return null;
    }
    public void ClearData()
    {
        this.GuideFinishedList.Clear();
        UIGuideButtonGameObject.Clear();
        GuideTaskExecuteList.Clear();
        TaskMrgData = null;
    }
}
