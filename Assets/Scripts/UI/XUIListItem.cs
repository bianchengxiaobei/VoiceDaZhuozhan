using UnityEngine;
using UnityEngine.UI;
using CaomaoFramework;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：XUIListItem
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class XUIListItem : MonoBehaviour
{
    public Toggle toggle;
    public Image sprite;
    public Texture texture;
    public Button button;
    /// <summary>
    /// 单个项所在列表的索引
    /// </summary>
    public int Index
    {
        get;set;
    }
    /// <summary>
    /// 单个项所在列表的id
    /// </summary>
    public int Id
    {
        get;set;
    }
    public XUIList ParentUIList
    {
        get;set;
    }
    /// <summary>
    /// 是否该单项被选中
    /// </summary>
    public bool IsSelected
    {
        get
        {
            return this.toggle != null && this.toggle.isOn;
        }
        set
        {
            if (this.toggle != null && this.toggle.isOn != value)
            {
                this.toggle.isOn = value;
                if (value)
                {
                    this.ParentUIList.SelectItem(this, false);
                }
                else
                {
                    this.ParentUIList.UnSelectItem(this, false);
                }
            }
        }
    }
    public void SetSprite(string spritePath)
    {
        if (this.sprite != null)
        {
            Sprite icon = Resources.Load<Sprite>(spritePath);
            this.sprite.sprite = icon;
        }
    }
    public void SetSprite(string id, string atla,string spriteName)
    {
        this.GetSprite(id).sprite = WWWResourceManager.Instance.LoadSpriteFormAtla(atla, spriteName);
    }
    public void SetText(string id,string content)
    {
        Transform label = this.transform.Find(id);
        if (label)
        {
            label.GetComponent<Text>().text = content;
        }
        else
        {
            Debug.LogError("没有找到Label:" + id);
        }
    }
    public Transform GetChild(string id)
    {
        Transform label = this.transform.Find(id);
        if (label)
        {
            return label;
        }
        else
        {
            Debug.LogError("没有找到Label:" + id);
            return null;
        }
    }
    public void SetTextColor(string id, Color color)
    {
        Transform label = this.transform.Find(id);
        if (label)
        {
            label.GetComponent<Text>().color = color;
        }
        else
        {
            Debug.LogError("没有找到Label:" + id);
        }
    }
    public Image GetSprite(string id)
    {
        Transform sprite = this.transform.Find(id);
        Image image = null;
        if (sprite)
        {
            image = sprite.GetComponent<Image>();
        }
        else
        {
            Debug.Log("没有找到Image:" + id);
        }
        return image;
    }
    public Text GetLabel(string id)
    {
        Transform text = this.transform.Find(id);
        Text label = null;
        if (text)
        {
            label = text.GetComponent<Text>();
        }
        else
        {
            Debug.Log("没有找到Text:" + id);
        }
        return label;
    }
    public Button GetButton(string id)
    {
        Transform btn = this.transform.Find(id);
        if (!btn.gameObject.activeSelf)
        {
            btn.gameObject.SetActive(true);
        }
        Button button = null;
        if (btn)
        {
            button = btn.GetComponent<Button>();
        }
        else
        {
            Debug.Log("没有找到Btn:" + id);
        }
        return button;
    }
    public void SetVisible(bool value)
    {
        if (this.gameObject.activeSelf != value)
        {
            this.gameObject.SetActive(value);
        }
    }
    public void SetVisible(string id, bool value)
    {
        Transform obj = this.transform.Find(id);
        if (obj != null)
        {
            obj.gameObject.SetActive(value);
        }
        else
        {
            Debug.LogError("找不到该物体的Id：" + id);
        }
    }
    private void Awake()
    {
        this.toggle = this.GetComponent<Toggle>();
        this.sprite = this.GetComponent<Image>();
        this.texture = this.GetComponent<Texture>();
        this.button = this.GetComponent<Button>();    
        if (this.toggle != null)
        {
            this.toggle.onValueChanged.AddListener(this.OnSelectStateChange);            
        }
        if (this.button != null)
        {
            this.button.onClick.AddListener(this.OnClick);
        }
    }
    private void Start()
    {
        if (this.toggle != null)
            this.toggle.group = transform.parent.GetComponent<ToggleGroup>();
    }
    private void OnSelectStateChange(bool value)
    {
        if (value)
        {
            this.ParentUIList.SelectItem(this, true);
        }
        else
        {
            this.ParentUIList.UnSelectItem(this, true);
        }
    }
    public void OnClick()
    {
        if (this.ParentUIList != null)
        {
            this.ParentUIList.OnClickXUIListItem(this);
        }
    }   
}
