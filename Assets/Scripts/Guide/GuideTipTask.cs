using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：GuideTipTask
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideTipTask : GuideTaskBase
{
    private GameObject m_oTip;
    public GuideTipTask(int taskId, EGuideTaskType type, GameObject parent) : base(taskId, type, parent)
    {

    }
    public override void EnterTask()
    {
        DataGuideTipInfo data = GameData<DataGuideTipInfo>.dataMap[this.TaskId];
        if (data != null)
        {
            WWWResourceManager.Instance.Load(data.TipPath, (asset) => 
            {
                if (asset != null)
                {
                    this.m_oTip = asset.Instantiate();
                    this.m_oTip.transform.SetParent(this.mRoot.transform);
                    this.m_oTip.transform.localPosition = data.LabelPos;
                    this.m_oTip.transform.localScale = Vector3.one;
                    this.m_oTip.GetComponentInChildren<Text>().text = data.Content;
                }
            });
        }
        else
        {
            this.FinishTask();
        }
    }
    public override void ExcuseTask()
    {
        
    }
    public override void ClearTask()
    {
        base.ClearTask();
        if (this.m_oTip != null)
        {
            GameObject.Destroy(this.m_oTip);
        }
    }
}
