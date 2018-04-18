using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：DlgLevel
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DlgLevel : UIBase
{
    public Button m_Button_Back;
    public Button m_Button_EnterGame;
    public XUIList m_List_Level;
    private int selectLevelId = int.MinValue;
    public DlgLevel()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgLevel.prefab";
        this.mELayer = EUILayer.Middle;
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgLevelShow, this.Show);
        EventDispatch.AddListener(Events.DlgLevelHide, this.Hide);
    }

    public override void OnDisable()
    {
        this.selectLevelId = int.MinValue;
    }

    public override void OnEnable()
    {
        this.m_Button_EnterGame.interactable = false;
        this.ShowLevel();
    }

    public override void Realse()
    {
        
    }

    protected override void InitWidget()
    {
        this.m_List_Level = this.mRoot.Find("list_level").GetComponent<XUIList>();
        this.m_Button_Back = this.mRoot.Find("bt_back").GetComponent<Button>();
        this.m_Button_EnterGame = this.mRoot.Find("bt_enter").GetComponent<Button>();       
        this.m_Button_Back.onClick.AddListener(this.OnclickBackButton);
        this.m_Button_EnterGame.onClick.AddListener(this.OnClickEnterGameButton);

        this.m_List_Level.RegisterListSelectEventHandler(this.OnSelectLevelListItem);
    }

    protected override void OnAddListener()
    {
        
    }

    protected override void OnRemoveListener()
    {
        
    }

    protected override void RealseWidget()
    {
        
    }
    public void OnclickBackButton()
    {
        this.Hide();
    }
    public void OnClickEnterGameButton()
    {
        if (this.selectLevelId > 0)
        {
            LevelManager.singleton.EnterLevel(this.selectLevelId);
            BattleController.singleton.EnterGameMain();
            ClientGameStateManager.singleton.ChangeGameState("GameMainState",CaomaoFramework.GameState.ELoadingStyle.LoadingNormal,()=> 
            {
                EventDispatch.Broadcast(Events.DlgLoadingAllFinished);
            });
        }
    }
    public void OnSelectLevelListItem(XUIListItem item)
    {
        if (item == null || item.Id <= 0)
        {
            return;
        }
        //如果还没有解锁
        if (!LevelManager.singleton.dicLevels[item.Id].valid && UnityMonoDriver.s_instance.ReleaseMode)
        {
            return;
        }
        this.selectLevelId = item.Id;
        this.m_Button_EnterGame.interactable = true;
    }
    public void ShowLevel()
    {
        int index = 0;
        foreach (var level in  LevelManager.singleton.dicLevels.Values)
        {
            XUIListItem item;
            if (index < this.m_List_Level.Count)
            {
                item = this.m_List_Level.GetItemByIndex(index);
            }
            else
            {
                item = this.m_List_Level.AddListItem();
            }
            if (item != null)
            {
                item.Id = level.levelId;
                if (level.valid)
                {
                    item.sprite.sprite = WWWResourceManager.Instance.LoadSpriteFormAtla("common1.ab", "levelItem");
                    item.SetText("lb_name", level.levelName);
                }
                else
                {
                    item.sprite.sprite = WWWResourceManager.Instance.LoadSpriteFormAtla("common1.ab", "unlock");
                    item.GetChild("lb_name").gameObject.SetActive(false);
                    if (UnityMonoDriver.s_instance.ReleaseMode)
                    {
                        item.toggle.interactable = false;
                    }
                }
            }
            index++;
        }     
    }
}
