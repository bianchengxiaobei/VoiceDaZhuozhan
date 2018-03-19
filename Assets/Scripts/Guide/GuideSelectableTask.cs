using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
using CaomaoFramework.Audio;
/*----------------------------------------------------------------
// 模块名：GuideSelectableTask
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideSelectableTask : GuideTaskBase
{
    public GameObject obj;
    public Button ok;
    public Button cacul;
    public GuideSelectableTask(int taskId, EGuideTaskType type, GameObject parent) : base(taskId, type, parent)
    {

    }
    public override void EnterTask()
    {
        WWWResourceManager.Instance.Load("Assets.Prefabs.Guis.UIGuideSelectable.prefab", (asset) => 
        {
            if (asset != null)
            {
                obj = asset.Instantiate();
                obj.transform.SetParent(this.mRoot.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                this.ok = obj.transform.Find("bt_sure").GetComponent<Button>();
                this.cacul = obj.transform.Find("bt_cacul").GetComponent<Button>();
                this.ok.onClick.AddListener(this.OnClickOkButton);
                this.cacul.onClick.AddListener(this.OnClickCaculButton);
            }
        });
    }
    public override void ClearTask()
    {
        if (ok != null)
        {
            ok.onClick.RemoveAllListeners();
        }
        if (cacul != null)
        {
            cacul.onClick.RemoveAllListeners();
        }
        if (this.obj != null)
        {
            GameObject.Destroy(this.obj);
        }
        base.ClearTask();
    }
    private void OnClickOkButton()
    {
        EventDispatch.Broadcast(Events.DlgTextEnterLearn);
        this.FinishTask();
        GuideModel.singleton.NowTaskId = 6004;
        EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
    }
    private void OnClickCaculButton()
    {
        if (this.obj != null)
        {
            GameObject.Destroy(this.obj);
        }
        this.FinishTask();
    }
}
