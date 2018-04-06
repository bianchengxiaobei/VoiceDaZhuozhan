using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
/*----------------------------------------------------------------
// 模块名：XUIList
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public enum ESortType
{
    Name,
    SilbingIndex
}
public class XUIList : MonoBehaviour
{
    private List<XUIListItem> m_listXUIListItem = new List<XUIListItem>();
    private List<XUIListItem> m_listXUIListItemSelected = new List<XUIListItem>();
    public bool MultiSelect;
    public ESortType sort = ESortType.Name;
    public GameObject PrefabListItem;
    private ListSelectEventHandler m_eventHandlerOnSelect;
    private ListSelectEventHandler m_eventHandlerOnUnSelect;
    private ListClickEventHandler m_eventHandlerOnClick;
    /// <summary>
    /// 列表中的项目数量
    /// </summary>
    public int Count
    {
        get
        {
            if (null == this.m_listXUIListItem)
            {
                return 0;
            }
            return this.m_listXUIListItem.Count;
        }
    }
    public void Awake()
    {
        //先清除
        this.m_listXUIListItem.Clear();
        this.m_listXUIListItemSelected.Clear();
        //然后初始化列表
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            XUIListItem component = child.GetComponent<XUIListItem>();
            if (null == component)
            {
                Debug.Log(string.Format("null == uiListItem. path ={0}", child.gameObject.name));
            }
            else
            {
                component.ParentUIList = this;
                this.m_listXUIListItem.Add(component);
            }
        }
        //列表根据名字来排序
        if (sort == ESortType.Name)
        {
            this.m_listXUIListItem.Sort(new Comparison<XUIListItem>(XUIList.SortByName));
        }
        else if (sort == ESortType.SilbingIndex)
        {
            this.m_listXUIListItem.Sort(new Comparison<XUIListItem>(XUIList.SortBySilbingIndex));
        }
        int num = 0;
        foreach (var current in this.m_listXUIListItem)
        {
            current.name = string.Format("{0:0000}", num);//名字设置成0001，0002这样的格式
            current.Index = num;//初始索引
            current.Id = num;//初始id
            if (current.IsSelected)
            {
                this.m_listXUIListItemSelected.Add(current);//如果选中就添加到选中列表中
            }
            num++;
        }
    }
    /// <summary>
    /// 选中该单项元素
    /// </summary>
    /// <param name="listItem"></param>
    /// <param name="bTrigerEvent"></param>
    public void SelectItem(XUIListItem listItem, bool bTrigerEvent)
    {
        if (null == listItem)
        {
            return;
        }
        if (this.m_listXUIListItemSelected.Contains(listItem))
        {
            return;
        }
        if (!this.MultiSelect)
        {
            this.m_listXUIListItemSelected.Clear();
        }
        this.m_listXUIListItemSelected.Add(listItem);
        if (this.m_eventHandlerOnSelect != null && bTrigerEvent)
        {
            this.m_eventHandlerOnSelect(listItem);
        }
    }
    public void UnSelectItem(XUIListItem listItem, bool bTrigerEvent)
    {
        if (null == listItem)
        {
            return;
        }
        if (!this.m_listXUIListItemSelected.Contains(listItem))
        {
            return;
        }
        this.m_listXUIListItemSelected.Remove(listItem);
        if (this.m_eventHandlerOnUnSelect != null && bTrigerEvent)
        {
            this.m_eventHandlerOnUnSelect(listItem);
        }
    }
    public void UnSelectAllItems(bool bTrigerEvent)
    {
        for (int i = 0; i < this.m_listXUIListItemSelected.Count; i++)
        {
            this.m_listXUIListItemSelected.RemoveAt(i);
            if (this.m_eventHandlerOnUnSelect != null && bTrigerEvent)
            {
                this.m_eventHandlerOnUnSelect(this.m_listXUIListItemSelected[i]);
            }
        }
    }
    public bool DelItemByIndex(int nIndex)
    {
        XUIListItem itemByIndex = this.GetItemByIndex(nIndex);
        return this.DelItem(itemByIndex);
    }
    public bool DelItem(XUIListItem xUIListItem)
    {
        if (null == xUIListItem)
        {
            return false;
        }
        this.m_listXUIListItemSelected.Remove(xUIListItem);
        int index = xUIListItem.Index;
        for (int i = index + 1; i < this.Count; i++)
        {
            this.m_listXUIListItem[i].name = string.Format("{0:0000}", i - 1);
            this.m_listXUIListItem[i].Index = i - 1;
        }
        this.m_listXUIListItem.Remove(xUIListItem);
        xUIListItem.gameObject.transform.parent = null;
        UnityEngine.Object.Destroy(xUIListItem.gameObject);
        return true;
    }
    public void DelAllItem()
    {
        foreach (var item in GetAllItem())
        {
            if (item != null)
            {
                this.DelItem(item);
            }
        }
    }
    public XUIListItem[] GetAllItem()
    {
        return this.m_listXUIListItem.ToArray();
    }
    /// <summary>
    /// 添加在Inspector窗口显示的物体到UI列表中
    /// </summary>
    /// <returns></returns>
    public XUIListItem AddListItem()
    {
        if (this.PrefabListItem != null)
        {
            return this.AddListItem(this.PrefabListItem);
        }
        return null;
    }
    public XUIListItem AddListItem(GameObject obj)
    {
        if (null != obj)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
            gameObject.name = string.Format("{0:0000}", this.Count);//格式是0001，0002......
            gameObject.transform.parent = transform;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localRotation = Quaternion.identity;
            XUIListItem item = gameObject.GetComponent<XUIListItem>();
            if (null == item)
            {
                Debug.LogError("null == uiListItem");
                item = gameObject.AddComponent<XUIListItem>();
            }
            item.Index = this.Count;
            item.Id = item.Index;
            item.ParentUIList = this;
            this.m_listXUIListItem.Add(item);
            return item;
        }
        else
        {
            Debug.Log("prefabItem == null");
        }
        return null;
    }
    /// <summary>
    /// 根据索引取得列表中的单项元素
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public XUIListItem GetItemByIndex(int index)
    {
        if (null == this.m_listXUIListItem)
        {
            return null;
        }
        if (0 > index || index >= this.m_listXUIListItem.Count)
        {
            return null;
        }
        return this.m_listXUIListItem[index];
    }
    /// 根据id取得列表中的单项元素
    /// </summary>
    /// <param name="unId"></param>
    /// <returns></returns>
    public XUIListItem GetItemById(int unId)
    {
        foreach (XUIListItem current in this.m_listXUIListItem)
        {
            if (unId == current.Id)
            {
                return current;
            }
        }
        return null;
    }
    /// <summary>
    /// 根据元素的名字先后顺序排序
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int SortByName(XUIListItem a, XUIListItem b)
    {
        return string.Compare(a.name, b.name);
    }

    public static int SortBySilbingIndex(XUIListItem a, XUIListItem b)
    {
        if (a.transform.GetSiblingIndex() > b.transform.GetSiblingIndex())
        {
            return 1;
        }
        else if (a.transform.GetSiblingIndex() < b.transform.GetSiblingIndex())
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    public void RegisterListSelectEventHandler(ListSelectEventHandler eventHandler)
    {
        this.m_eventHandlerOnSelect = eventHandler;
    }
    public void RegisterListUnSelectEventHandler(ListSelectEventHandler eventHandler)
    {
        this.m_eventHandlerOnUnSelect = eventHandler;
    }
    public void RegisterListClickEventHandler(ListClickEventHandler eventHandler)
    {
        this.m_eventHandlerOnClick = eventHandler;
    }
    public void OnClickXUIListItem(XUIListItem item)
    {
        if (this.m_eventHandlerOnClick != null)
        {
            this.m_eventHandlerOnClick(item);
        }
    }
}
public delegate void ListSelectEventHandler(XUIListItem uiListItem);
public delegate void ListClickEventHandler(XUIListItem uiListItem);
