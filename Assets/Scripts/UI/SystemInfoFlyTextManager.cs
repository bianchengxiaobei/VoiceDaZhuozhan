using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
#region 模块信息
/*----------------------------------------------------------------
// 模块名SystemInfoFlyTextManager
// 创建者：chen
// 修改者列表：
// 创建日期：2017.3.6
// 模块描述：系统提示消息漂浮管理器
//----------------------------------------------------------------*/
#endregion
/// <summary>
/// 系统提示消息漂浮管理器
/// </summary>
internal class SystemInfoFlyTextManager : FlyTextManagerBase,IFlyTextManager
{
    private Text time;
    public SystemInfoFlyTextManager(XUIList list) :base(list)
    {
        this.m_fTotalTime = -1f;
    }
    protected override void InitFlyText(FlyTextEntity flyText, string strText, int targetBeast)
    {
        base.InitFlyText(flyText, strText, targetBeast);
        flyText.Label.text = strText;
        flyText.FlyTextItem.gameObject.SetActive(false);
        flyText.FlyTextItem.gameObject.SetActive (true);
        time = flyText.FlyTextItem.GetLabel("lb_leftTime");
    }
    public override FlyTextEntity Add(string strText, int targetBeastId)
    {
        FlyTextEntity flyTextEntity = null;
        if (this.m_flyTextList.First != null)
        {
            flyTextEntity = this.m_flyTextList.First.Value;
            flyTextEntity.Active = true;
        }
        else
        {
            XUIListItem item = this.m_uiList.AddListItem();
            if (item != null)
            {
                flyTextEntity = new FlyTextEntity(item, targetBeastId);
                this.m_flyTextList.AddFirst(flyTextEntity);
            }
        }
        this.InitFlyText(flyTextEntity, strText, targetBeastId);
        return flyTextEntity;
    }
    protected override void Translate(ref FlyTextEntity flyText, float fElapseTime)
    {
        if (time != null)
        {
            int time1 = (int)(this.m_fTotalTime - fElapseTime);
            string time2 = string.Format("({0})", time1);
            time.text = time2.ToString();
        }
            
    }
}
