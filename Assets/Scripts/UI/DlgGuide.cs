using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using System;

public class DlgGuide : UIBase
{
    public DlgGuide()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgGuide.prefab";
        this.mELayer = EUILayer.Top;
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgPlayGuideShow, this.Show);
        EventDispatch.AddListener(Events.DlgPlayGuideHide, this.Hide);
    }

    public override void OnDisable()
    {
        EventDispatch.RemoveListener<EGuideTaskType, int>(Events.DlgGuideChildTaskFinished, this.OnFinishChildTask);
    }

    public override void OnEnable()
    {
        
    }

    public override void Realse()
    {
        EventDispatch.RemoveListener(Events.DlgPlayGuideShow, this.Show);
        EventDispatch.RemoveListener(Events.DlgPlayGuideHide, this.Hide);
    }

    protected override void InitWidget()
    {
        this.mRoot.Find("sp_mask").gameObject.SetActive(true);
        EventDispatch.AddListener(Events.DlgGuideExecuteNextTask, this.ExecuteNextGuide);
        EventDispatch.AddListener<EGuideTaskType, int>(Events.DlgGuideChildTaskFinished, this.OnFinishChildTask);
        if (GuideModel.singleton.GuideTaskExecuteList.Count == 0)
        {
            this.ExecuteNextGuide();
        }
    }

    protected override void OnAddListener()
    {
        
    }

    protected override void OnRemoveListener()
    {
        
    }

    protected override void RealseWidget()
    {
        
    }
    private void ExecuteNextGuide()
    {
        int taskId = GuideModel.singleton.NowTaskId;
        if (!GameData<DataGuideParentTaskInfo>.dataMap.ContainsKey(taskId))
        {
            return;
        }
        List<int> idList = GameData<DataGuideParentTaskInfo>.dataMap[taskId].ChildTaskId;
        List<int> typeList = GameData<DataGuideParentTaskInfo>.dataMap[taskId].ChildTaskType;
        for (int i = 0; i < typeList.Count; i++)
        {
            GuideTaskBase task = null;
            EGuideTaskType eType = (EGuideTaskType)typeList[i];
            switch (eType)
            {
                case EGuideTaskType.ForceClickTask:
                    task = new GuideForceClick(idList[i], eType, this.mRoot.gameObject);
                    break;
                case EGuideTaskType.TipTask:
                    task = new GuideTipTask(idList[i], eType, this.mRoot.gameObject);
                    break;
                case EGuideTaskType.PopTipWindowTask:
                    task = new GuidePopTipWindowTask(idList[i], eType, this.mRoot.gameObject);
                    break;
                case EGuideTaskType.ShowTipTask:
                    task = new GuideShowTipContinueTask(idList[i], eType, this.mRoot.gameObject);
                    break;
                case EGuideTaskType.SelectableTask:
                    task = new GuideSelectableTask(idList[i], eType, this.mRoot.gameObject);
                    break;
            }
            task.EnterTask();
            GuideModel.singleton.GuideTaskExecuteList.Add(task);
        }
    }
    private void OnFinishChildTask(EGuideTaskType type, int taskId)
    {
        GuideController.singleton.OnFinishedChildTask(type, taskId);
    }
    public override void Update(float deltaTime)
    {

        if (GuideModel.singleton.GuideTaskExecuteList.Count > 0)
        {
            for (int i = 0; i < GuideModel.singleton.GuideTaskExecuteList.Count; i++)
            {
                GuideModel.singleton.GuideTaskExecuteList[i].ExcuseTask();
            }
        }
    }
}
