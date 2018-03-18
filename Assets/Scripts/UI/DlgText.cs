using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
/*----------------------------------------------------------------
// 模块名：DlgText
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DlgText : UIBase
{
    public XUIList m_List_Skills;
    public GameObject SKillView;
    public GameObject MySelf;
    public GameObject SkillLearn;
    public Image m_Iamge_SkillIcon;
    public Text m_Text_SkillInfo;
    public Text m_Text_SkillName;
    public Button m_Button_Close;

    private int selectSkillId = int.MinValue;
    public DlgText()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgTest.prefab";
        this.mELayer = EUILayer.Middle;
        this.mResident = false;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgTextShow, this.Show);
        EventDispatch.AddListener(Events.DlgTextHide, this.Hide);
    }

    public override void OnDisable()
    {
        
    }

    public override void OnEnable()
    {
        this.SKillView.SetActive(true);
        this.SkillLearn.SetActive(false);
        this.MySelf.SetActive(false);
        this.ShowLockedSkill();
    }

    public override void Realse()
    {
        
    }

    protected override void InitWidget()
    {
        this.SKillView = this.mRoot.Find("SkillView").gameObject;
        this.SkillLearn = this.mRoot.Find("SkillLearn").gameObject;
        this.MySelf = this.mRoot.Find("MySelf").gameObject;
        this.m_Iamge_SkillIcon = this.mRoot.Find("SkillLearn/SkillIcon").GetComponent<Image>();
        this.m_Text_SkillInfo = this.mRoot.Find("SkillLearn/SkillInfo/lb_info").GetComponent<Text>();
        this.m_Text_SkillName = this.mRoot.Find("SkillLearn/SkillName/Text").GetComponent<Text>();
        this.m_Button_Close = this.mRoot.Find("SkillView/bt_close").GetComponent<Button>();
        this.m_List_Skills = this.mRoot.Find("SkillView/Viewport/Content").GetComponent<XUIList>();
        this.m_List_Skills.RegisterListSelectEventHandler(this.OnSelectSkillItem);
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener(Events.DlgTextEnterLearn, this.GuideToEnterLearn);
    }

    protected override void OnRemoveListener()
    {
        EventDispatch.RemoveListener(Events.DlgTextEnterLearn, this.GuideToEnterLearn);
    }

    protected override void RealseWidget()
    {
        
    }
    public void ShowLockedSkill()
    {
        List<GameSkillBase> soreSkills = new List<GameSkillBase>();
        if (SkillManager.singleton.skills.Count > 0)
        {
            foreach (var skillconfig in SkillManager.singleton.skills)
            {
                if (skillconfig.Value.bLocked || skillconfig.Value.skillConfig.skillId == 5)
                {
                    soreSkills.Insert(0,skillconfig.Value);
                    continue;
                }
                soreSkills.Add(skillconfig.Value);
            }
        }
        else
        {
            Debug.LogError("skills == null");
            return;
        }
        for (int i = 0; i < soreSkills.Count; i++)
        {
            XUIListItem item = null;
            if (i < this.m_List_Skills.Count)
            {
                item = this.m_List_Skills.GetItemByIndex(i);
            }
            else
            {
                item = this.m_List_Skills.AddListItem();
            }
            if (item != null)
            {
                item.Id = soreSkills[i].skillConfig.skillId;
                if (item.Index == 0)
                {
                    GuideController.singleton.AddGuideEventButton(item.gameObject);
                }
                item.SetSprite("sp_icon", "common1.ab", soreSkills[i].skillConfig.iconPath);
                item.SetText("lb_name", soreSkills[i].skillConfig.skillName);
                bool hasLocked = soreSkills[i].bLocked;
                item.GetChild("sp_lock").gameObject.SetActive(!hasLocked);
                item.toggle.interactable = hasLocked;
                string btName = hasLocked ? "学习" : "解锁";
                item.SetText("bt_click/Text", btName);
                item.GetButton("bt_click").onClick.AddListener(()=> 
                {
                    if (hasLocked)
                    {
                        //进入学习
                        this.EnterLearn(item.Id);
                    }
                    else
                    {
                        //进入解锁
                        this.EnterLock(item.Id);
                    }
                });
            }
        }
    }
    public void EnterLearn(int skillId)
    {
        this.SKillView.SetActive(false);
        this.MySelf.SetActive(true);
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillId);
        if (skill != null)
        {
            this.m_Iamge_SkillIcon.sprite = WWWResourceManager.Instance.LoadSpriteFormAtla("common1.ab", skill.skillConfig.iconPath);
            this.m_Text_SkillInfo.text = skill.skillConfig.skillInfo;
            this.m_Text_SkillName.text = skill.skillConfig.skillName;
            this.SkillLearn.SetActive(true);
        }
    }
    public void GuideToEnterLearn()
    {
        this.EnterLearn(this.selectSkillId);
    }
    public void EnterLock(int skillId)
    {
        if (!GuideModel.singleton.bIsGuideAllComp)
        {

        }
    }
    public void OnSelectSkillItem(XUIListItem list)
    {
        if (list == null && list.Id <= 0)
        {
            return;
        }
        this.selectSkillId = list.Id;
    }
}
