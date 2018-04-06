using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CaomaoFramework;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：FlyTextEntity 
// 创建者：chen
// 修改者列表：
// 创建日期：2016.4.23
// 模块描述：浮动文字实体类
//----------------------------------------------------------------*/
#endregion
/// <summary>
/// 浮动文字实体类
/// </summary>
internal class FlyTextEntity
{
    private Vector3 m_posStart;
    private float m_fTimeStart;
    private XUIListItem m_uiListItem;
    private Text m_uiLabel;
    private long m_unTargetBeastId;
    private bool m_bActive;
    private Image m_uiSprite;
    public XUIListItem FlyTextItem
    {
        get
        {
            return this.m_uiListItem;
        }
    }
    public EntityParent Target
    {
        get; set;
    }
    public bool Active
    {
        get
        {
            return this.m_bActive;
        }
        set
        {
            this.m_bActive = value;
            this.m_uiListItem.SetVisible(value);
        }
    }
    public Vector3 PosStart
    {
        get
        {
            return this.m_posStart;
        }
        set
        {
            this.m_posStart = value;
        }
    }
    public float TimeStart
    {
        get
        {
            return this.m_fTimeStart;
        }
        set
        {
            this.m_fTimeStart = value;
        }
    }
    public Transform Transform
    {
        get
        {
            return this.m_uiListItem.transform;
        }
    }
    public Text Label
    {
        get
        {
            if (null != this.m_uiLabel)
            {
                return this.m_uiLabel;
            }
            return null;
        }
    }
    public Image Sprite
    {
        get
        {
            if (null != this.m_uiSprite)
            {
                return this.m_uiSprite;
            }
            return null;
        }
    }
    public FlyTextEntity(XUIListItem flyTextItem, long unTargetHeroId)
    {
        this.m_bActive = true;
        this.m_unTargetBeastId = unTargetHeroId;
        this.m_uiListItem = flyTextItem;
        this.m_uiLabel = this.m_uiListItem.GetLabel("lb_flytext");
        this.m_uiSprite = this.m_uiListItem.GetSprite("sp_bg");
        if (null == this.m_uiLabel)
        {        
            Debug.Log("null == m_uiLabel");
        }
        if (null == this.m_uiSprite)
        {
            Debug.Log("null == this.m_uiSprite");
        }
    }
}