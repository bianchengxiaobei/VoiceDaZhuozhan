using UnityEngine;
using System.Collections.Generic;
using System;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：CuideController
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideController : Singleton<GuideController>
{
    public void AddGuideEventButton(GameObject obj)
    {
        if (obj == null || GuideModel.singleton.UIGuideButtonGameObject.ContainsKey(obj.name))
        {
            return;
        }
        GuideModel.singleton.UIGuideButtonGameObject.Add(obj.name, obj);
        //通知事件
        EventDispatch.Broadcast(Events.DlgAddGuideEvent, obj);
    }
    public void AddGuideSprite(GameObject obj)
    {
        if (obj == null || GuideModel.singleton.UIGuideSpriteGameObject.ContainsKey(obj.name))
        {
            return;
        }
        GuideModel.singleton.UIGuideSpriteGameObject.Add(obj.name, obj);
        //通知事件
        //EventDispatch.Broadcast(Events.DlgAddGuideEvent, obj);
    }
    /// <summary>
    /// 开始改向导模块
    /// </summary>
    public void StartModelTask(EGuideStepInfo modelId)
    {
        foreach (var item in GameData<DataGuideParentTaskInfo>.dataMap)
        {
            if (modelId == (EGuideStepInfo)item.Value.TaskType)
            {
                GuideModel.singleton.NowTaskId = item.Key;
                Debug.Log("NowTaskId:" + GuideModel.singleton.NowTaskId);
                return;
            }
        }
    }
    /// <summary>
    /// 在收到服务器发送的已经完成的任务Id列表
    /// </summary>
    /// <param name="finishList"></param>
    public void GuideFinishModelList(List<int> finishList)
    {
        GuideModel.singleton.GuideFinishedList.Clear();
        GuideModel.singleton.GuideFinishedList.AddRange(finishList);
        GuideModel.singleton.CurrentTaskModelId = EGuideStepInfo.GuideStepNull;
        foreach (EGuideStepInfo step in Enum.GetValues(typeof(EGuideStepInfo)))
        {
            if (step == EGuideStepInfo.GuideStepNull)
            {
                continue;
            }
            if (!GuideModel.singleton.GuideFinishedList.Contains((int)step))
            {
                GuideModel.singleton.CurrentTaskModelId = step;
                break;
            }
        }
        if (GuideModel.singleton.CurrentTaskModelId != EGuideStepInfo.GuideStepNull)
        {
            this.StartModelTask(GuideModel.singleton.CurrentTaskModelId);
            //显示Guide的UI
            EventDispatch.Broadcast(Events.DlgPlayGuideShow);
        }
    }
    public void OnFinishedChildTask(EGuideTaskType type, int taskId)
    {
        GuideModel.singleton.TaskMrgData = GameData<DataGuideParentTaskInfo>.dataMap[GuideModel.singleton.NowTaskId];
        if (GuideModel.singleton.TaskMrgData != null)
        {
            if (GuideModel.singleton.TaskMrgData.EndTaskChildId == taskId)
            {
                this.OnChangeToNextTask();
            }
        }
    }
    public void OnChangeToNextTask()
    {
        this.ClearExecuteGuideTask();
        //如果完成的任务是模块的最后一个任务，告诉服务器
        if (GuideModel.singleton.TaskMrgData.Moduleend)
        {
            //保存下
            string finished = PlayerPrefs.GetString(CommonDefineBase.GuideFinish);
            if (string.IsNullOrEmpty(finished))
            {
                finished = GuideModel.singleton.TaskMrgData.TaskType.ToString();
            }
            else
            {
                finished += "," + GuideModel.singleton.TaskMrgData.TaskType;
            }
            PlayerPrefs.SetString(CommonDefineBase.GuideFinish,finished);
            if ((EGuideStepInfo)GuideModel.singleton.TaskMrgData.TaskType == EGuideStepInfo.WatchAds)
            {
                //结束
                GuideModel.singleton.bIsGuideAllComp = true;
                GuideModel.singleton.bIsGuideBattle = false;
                GuideModel.singleton.ClearData();
                string finished2 = PlayerPrefs.GetString(CommonDefineBase.GuideFinish);
                finished2 += ",ok";
                PlayerPrefs.SetString(CommonDefineBase.GuideFinish, finished2);
            }
        }
        if (GuideModel.singleton.bIsGuideAllComp)
        {
            //不显示GUideUI
            this.Exit();
            return;
        }
        if (GuideModel.singleton.TaskMrgData.NextTaskId == -1)
        {
            return;
        }
        GuideModel.singleton.NowTaskId = GuideModel.singleton.TaskMrgData.NextTaskId;
        if (!GameData<DataGuideParentTaskInfo>.dataMap.ContainsKey(GuideModel.singleton.NowTaskId))
        {
            return;
        }
        GuideModel.singleton.TaskMrgData = GameData<DataGuideParentTaskInfo>.dataMap[GuideModel.singleton.NowTaskId];
        if (GuideModel.singleton.TaskMrgData == null)
        {
            return;
        }
        EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
    }
    private void ClearExecuteGuideTask()
    {
        foreach (var task in GuideModel.singleton.GuideTaskExecuteList)
        {
            task.ClearTask();
        }
        GuideModel.singleton.GuideTaskExecuteList.Clear();
    }
    public void Exit()
    {
        EventDispatch.Broadcast(Events.DlgPlayGuideHide);
    }
}
