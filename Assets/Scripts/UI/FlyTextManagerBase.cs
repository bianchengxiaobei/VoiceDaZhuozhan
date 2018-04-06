using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：FlyTextManagerBase 
// 创建者：chen
// 修改者列表：
// 创建日期：2016.4.23
// 模块描述：浮动文字管理器抽象类
//----------------------------------------------------------------*/
#endregion
/// <summary>
/// 浮动文字管理器抽象类
/// </summary>
internal abstract class FlyTextManagerBase 
{
	#region 字段
    protected XUIList m_uiList;
    protected float m_fTotalTime = 2f;
    protected LinkedList<FlyTextEntity> m_flyTextList = new LinkedList<FlyTextEntity>();
    protected bool m_bShow;
	#endregion
	#region 属性
    public LinkedList<FlyTextEntity> FlyTexts
    {
        get { return this.m_flyTextList; }
    }
	#endregion
	#region 构造方法
    public FlyTextManagerBase(XUIList uiList)
    {
        this.m_uiList = uiList;
    }
	#endregion
	#region 公共方法
    public virtual FlyTextEntity Add(string strText, int targetHeroId)
    {
       Debug.Log("AddFlyText" + strText);
        FlyTextEntity flyTextEntity = null;
        if (this.m_flyTextList.Last != null && !this.m_flyTextList.Last.Value.Active)
        {
            flyTextEntity = this.m_flyTextList.Last.Value;
            this.m_flyTextList.RemoveLast();
            flyTextEntity.Active = true;
        }
        else 
        {
            XUIListItem item = this.m_uiList.AddListItem();
            if (item != null)
            {
                flyTextEntity = new FlyTextEntity(item, targetHeroId);
            }
            else 
            {
                XLog.Log.Error("Item == null");
            }
        }
        this.InitFlyText(flyTextEntity, strText, targetHeroId);
        this.m_flyTextList.AddFirst(flyTextEntity);
        return flyTextEntity;
    }
    /// <summary>
    /// 浮动文字移动更新
    /// </summary>
    public virtual void Update()
    {
        float time = Time.time;
        LinkedListNode<FlyTextEntity> next;
        for (LinkedListNode<FlyTextEntity> linkedListNode = this.m_flyTextList.First; linkedListNode != null; linkedListNode = next)
        {
            next = linkedListNode.Next;
            //如果是外面手动关闭的话
            if (!linkedListNode.Value.Active)
            {
                //FlyTextEntity value1 = linkedListNode.Value;
                //this.m_flyTextList.Remove(linkedListNode);
                //this.m_flyTextList.AddLast(value1);
                break;
            }
            //如果是无限时间的话
            if (this.m_fTotalTime == -1)
            {
                continue;
            }
            FlyTextEntity value = linkedListNode.Value;
            float num = time - value.TimeStart;
            if (num < this.m_fTotalTime)
            {
                this.Translate(ref value, num);
            }
            else
            {
                if (this.m_flyTextList.Count <= 5)
                {
                    value.Active = false;
                    this.m_flyTextList.Remove(linkedListNode);
                    this.m_flyTextList.AddLast(value);
                }
                else
                {
                    this.m_uiList.DelItemByIndex(value.FlyTextItem.Index);
                    this.m_flyTextList.Remove(linkedListNode);
                }
            }
        }
    }
    /// <summary>
    /// 初始化浮动文字
    /// </summary>
    /// <param name="flyText"></param>
    /// <param name="strText"></param>
    /// <param name="targetHeroId"></param>
    protected virtual void InitFlyText(FlyTextEntity flyText, string strText, int targetHeroId)
    {
        EntityParent hero = null;
        if (targetHeroId > 0)
            hero = PlayerManager.singleton.GetEntity(targetHeroId);
        if (flyText.Label != null)
            flyText.Label.text = strText;
        flyText.TimeStart = Time.time;
        flyText.Target = hero;
        if (hero != null)
        {
            Vector3 realPos = hero.Transform.position;
            realPos.y += 2.5f;
            flyText.PosStart = realPos;
        }
    }
    protected virtual void Translate(ref FlyTextEntity flyText, float fElapseTime)
    {

    }
	#endregion
	#region 私有方法
	#endregion
	#region 析构方法
	#endregion
}
