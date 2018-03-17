using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
using DG.Tweening;
/*----------------------------------------------------------------
// 模块名：GuidePopTipWindowTask
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuidePopTipWindowTask : GuideTaskBase
{
    private DataPopTipWindowTaskInfo data;
    private GameObject m_oPopTipWin;
    private Tweener scaleTweener;
    public GuidePopTipWindowTask(int taskId, EGuideTaskType type, GameObject parent) : base(taskId, type, parent)
    {

    }
    public override void EnterTask()
    {
        data = GameData<DataPopTipWindowTaskInfo>.dataMap[this.TaskId];
        if (data == null)
        {
            this.FinishTask();
            return;
        }
        WWWResourceManager.Instance.Load(data.Path, (asset) => 
        {
            if (asset != null)
            {
                this.m_oPopTipWin = asset.Instantiate();
                this.m_oPopTipWin.transform.SetParent(this.mRoot.transform);
                this.m_oPopTipWin.transform.localPosition = data.Pos;
                this.m_oPopTipWin.transform.localScale = Vector3.one;
                this.m_oPopTipWin.GetComponentInChildren<Text>().text = data.Content;

                scaleTweener = this.m_oPopTipWin.transform.DOScale(data.ToScale, data.Duration);
            }
        });
    }
    public override void ClearTask()
    {
        if (this.m_oPopTipWin != null)
        {
            GameObject.Destroy(this.m_oPopTipWin);
        }
        base.ClearTask();
        this.scaleTweener.Kill();
        this.scaleTweener = null;
        this.data = null ;
    }
}
