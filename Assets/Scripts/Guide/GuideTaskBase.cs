using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：GuideTaskBase
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 向导（动作）基类
/// </summary>
public class GuideTaskBase
{
    public int TaskId;
    protected GameObject mRoot;
    protected bool m_bFinished = false;
    public EGuideTaskType taskType;
    protected bool bTaskCoolDown;
    protected float m_fTaskCDtime;
    protected float m_fTaskTime;

    public GuideTaskBase(int taskId, EGuideTaskType type, GameObject parent)
    {
        this.TaskId = taskId;
        this.taskType = type;
        this.mRoot = parent;
    }

    public bool IsFinished
    {
        get { return this.m_bFinished; }
    }

    public virtual void EnterTask()
    {

    }
    /// <summary>
    /// 执行任务
    /// </summary>
    public virtual void ExcuseTask()
    {
        //if (!bTaskCoolDown)
        //{
        //    return;
        //}
        //if (Time.realtimeSinceStartup - m_fTaskTime >= m_fTaskCDtime)
        //{
        //    bTaskCoolDown = false;
        //}
    }

    //任务结束广播消息
    public virtual void FinishTask()
    {
        if (m_bFinished)
        {
            return;
        }
        m_bFinished = true;
        EventDispatch.Broadcast<EGuideTaskType,int>(Events.DlgGuideChildTaskFinished, this.taskType, this.TaskId);
    }
    /// <summary>
    /// 清理任务
    /// </summary>
    public virtual void ClearTask()
    {

    }
}
