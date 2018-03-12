using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：DefaultUI
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DefaultUI : MonoBehaviour
{
    private static DefaultUI m_oInstance;
    public static DefaultUI Instance
    {
        get
        {
            if (m_oInstance != null)
            {
                return m_oInstance;
            }
            return null;
        }  
    }

    public Text m_Label_Tip;
    public Slider m_Slider_Progress;
    private void Awake()
    {
        m_oInstance = this.transform.GetComponent<DefaultUI>();
        this.m_Label_Tip = this.transform.Find("lb_tip").GetComponent<Text>();
        this.m_Slider_Progress = this.transform.Find("pg_progress").GetComponent<Slider>();
    }
    public void SetTipContent(string value)
    {
        this.m_Label_Tip.text = value;
    }
    public void SetProgress(float value)
    {
        this.m_Slider_Progress.value = value;
    }
    public void ShowProgressBar(bool value)
    {
        if (this.m_Slider_Progress != null)
        {
            this.m_Slider_Progress.gameObject.SetActive(value);
        }
        if (this.m_Label_Tip != null)
        {
            this.m_Label_Tip.gameObject.SetActive(value);
        }
    }
    public void Release()
    {
        Destroy(this.gameObject);
        this.m_Label_Tip = null;
        this.m_Slider_Progress = null;
        m_oInstance = null;
        Resources.UnloadUnusedAssets();
    }
}
