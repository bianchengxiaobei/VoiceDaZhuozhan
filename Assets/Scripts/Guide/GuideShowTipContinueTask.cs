using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：GuideShowTipContinueTask
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class GuideShowTipContinueTask : GuideTaskBase
{
    private DataGuideShowTipContinueInfo data;
    private GameObject tipButton;
    private Text tip_Label;
    private GameObject curShowSprite;
    private GameObject Black;
    private Transform spriteTempParent;
    private int nextShowId;
    public GuideShowTipContinueTask(int taskId, EGuideTaskType type, GameObject parent) : base(taskId, type, parent)
    {
        this.nextShowId = taskId;
    }
    public override void EnterTask()
    {
        data = GameData<DataGuideShowTipContinueInfo>.dataMap[this.nextShowId];
        if (data != null)
        {
            if (this.tipButton == null)
            {
                WWWResourceManager.Instance.Load(data.TipBtnPath, (assets) =>
                {
                    if (assets != null)
                    {
                        tipButton = assets.Instantiate();
                        tipButton.transform.SetParent(this.mRoot.transform);
                        tipButton.transform.localPosition = data.TipBtnPos;
                        tipButton.transform.localScale = Vector3.one;
                        tip_Label = tipButton.transform.Find("lb_tip").GetComponent<Text>();
                        tip_Label.text = data.TipContent;
                        this.tipButton.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (data.NextId != -1)
                            {
                                this.nextShowId = data.NextId;
                                this.FinishedChild();
                                this.EnterTask();
                            }
                            else
                            {
                                this.FinishedChild();
                                this.FinishTask();
                                return;
                            }
                        });
                    }
                });
            }
            else
            {
                tipButton.transform.localPosition = data.TipBtnPos;
                tip_Label.text = data.TipContent;
            }
            if (this.Black == null)
                this.Black = this.mRoot.transform.Find("sp_black").gameObject;
            StartShow();
        }
        else
        {
            this.FinishTask();
            return;
        }
    }
    private void StartShow()
    {
        if (!string.IsNullOrEmpty(data.SpriteName))
        {
            this.curShowSprite = GuideModel.singleton.GetUIGuideSpriteGameObject(data.SpriteName);
            if (curShowSprite == null)
            {
                return;
            }
            this.spriteTempParent = this.curShowSprite.transform.parent;
            this.curShowSprite.transform.SetParent(this.Black.transform);
            if (data.SpritePos != Vector3.zero)
            {
                this.curShowSprite.transform.localPosition = data.SpritePos;                
            }
            this.curShowSprite.transform.localScale = Vector3.one;
            if (!this.Black.activeSelf)
            {
                this.Black.SetActive(true);
            }
        }
    }
    private void FinishedChild()
    {
        this.curShowSprite.transform.SetParent(this.spriteTempParent);
        this.spriteTempParent = null;
        this.curShowSprite = null;
    }
    public override void FinishTask()
    {
        base.FinishTask();
    }
    public override void ClearTask()
    {
        if (this.tipButton)
        {
            GameObject.Destroy(this.tipButton);
        }
        if (data != null)
        {
            data = null;
        }
        this.curShowSprite = null;
        this.tip_Label = null;
        if (this.Black)
        {
            this.Black.SetActive(false);
        }
    }
}
