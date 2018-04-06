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
    public GameObject SpeakToken;
    public GameObject ButtonSpeak;
    public GameObject SpeakMask;
    public Image m_Iamge_SkillIcon;
    public Text m_Text_SkillInfo;
    public Text m_Text_SkillName;
    public Text m_Text_Token;
    public Button m_Button_Close;
    public Button m_Button_Test;
    public Button m_Button_Learn;
    public Button m_Button_Back;
    private int selectSkillId = int.MinValue;
    private EntityLearn entity;
    private uint ShowTokenTimerId = uint.MinValue;
    private TestParse parse = TestParse.SkillView;
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
        this.entity = null;
        VoiceManager.Instance.UnRegisterCallback();
    }

    public override void OnEnable()
    {
        this.SKillView.SetActive(true);
        this.SkillLearn.SetActive(false);
        this.MySelf.SetActive(false);
        this.SpeakToken.SetActive(false);
        this.ButtonSpeak.SetActive(false);
        this.SpeakMask.SetActive(false);
        this.m_Button_Back.gameObject.SetActive(false);
        this.ShowLockedSkill();
        VoiceManager.Instance.RegisterCallback(this.CastSkill,this.CastSkillError);
    }

    public override void Realse()
    {
        
    }

    protected override void InitWidget()
    {
        this.SKillView = this.mRoot.Find("SkillView").gameObject;
        this.SkillLearn = this.mRoot.Find("SkillLearn").gameObject;
        this.MySelf = this.mRoot.Find("MySelf").gameObject;
        this.ButtonSpeak = this.mRoot.Find("bt_voice").gameObject;
        GuideController.singleton.AddGuideEventButton(this.ButtonSpeak);
        this.SpeakMask = this.mRoot.Find("sp_black").gameObject;
        this.entity = new EntityLearn();
        this.entity.Init(this.MySelf.transform);
        this.SpeakToken = this.mRoot.Find("sp_token").gameObject;
        this.m_Iamge_SkillIcon = this.mRoot.Find("SkillLearn/SkillIcon").GetComponent<Image>();
        this.m_Text_SkillInfo = this.mRoot.Find("SkillLearn/SkillInfo/lb_info").GetComponent<Text>();
        this.m_Text_SkillName = this.mRoot.Find("SkillLearn/SkillName/Text").GetComponent<Text>();
        this.m_Text_Token = this.mRoot.Find("sp_token/lb_info").GetComponent<Text>();
        this.m_Button_Close = this.mRoot.Find("SkillView/bt_close").GetComponent<Button>();
        this.m_Button_Close.onClick.AddListener(this.OnClickButtonClose);
        GuideController.singleton.AddGuideEventButton(this.m_Button_Close.gameObject);
        this.m_List_Skills = this.mRoot.Find("SkillView/Viewport/Content").GetComponent<XUIList>();
        this.m_List_Skills.RegisterListSelectEventHandler(this.OnSelectSkillItem);
        this.m_Button_Test = this.mRoot.Find("SkillLearn/bt_speakTest").GetComponent<Button>();
        this.m_Button_Test.onClick.AddListener(this.OnClickButtonSpeakTest);
        GuideController.singleton.AddGuideEventButton(this.m_Button_Test.gameObject);
        this.m_Button_Learn = this.mRoot.Find("SkillLearn/bt_learn").GetComponent<Button>();
        this.m_Button_Learn.onClick.AddListener(this.OnClickButtonSelfSpeak);
        GuideController.singleton.AddGuideEventButton(this.m_Button_Learn.gameObject);
        this.m_Button_Back = this.mRoot.Find("bt_back").GetComponent<Button>();
        this.m_Button_Back.onClick.AddListener(this.OnClickButtonBack);
        GuideController.singleton.AddGuideEventButton(this.m_Button_Back.gameObject);

    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener(Events.DlgTextEnterLearn, this.GuideToEnterLearn);
        EventDispatch.AddListener<bool>(Events.DlgTextShowMask, this.ShowMask);
    }

    protected override void OnRemoveListener()
    {
        EventDispatch.RemoveListener(Events.DlgTextEnterLearn, this.GuideToEnterLearn);
        EventDispatch.RemoveListener<bool>(Events.DlgTextShowMask, this.ShowMask);
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

                item.SetSprite("sp_icon", "common1.ab", soreSkills[i].skillConfig.skillName);
                item.SetText("lb_name", soreSkills[i].skillConfig.skillName);
                bool hasLocked = soreSkills[i].bLocked;
                item.GetChild("sp_lock").gameObject.SetActive(!hasLocked);
                item.toggle.interactable = hasLocked;
                string btName = hasLocked ? "学习" : "解锁";
                item.SetText("bt_click/Text", btName);
                Button clickButton = item.GetButton("bt_click");
                if (item.Index == 0)
                {
                    GuideController.singleton.AddGuideEventButton(clickButton.gameObject);
                }
                clickButton.onClick.AddListener(()=> 
                {
                    this.m_List_Skills.SelectItem(item, true);
                    
                    if (SkillManager.singleton.GetSkill(item.Id).bLocked)
                    {
                        //进入学习
                        this.EnterLearn(item.Id,false);
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
    public void EnterLearn(int skillId, bool bSelect = true)
    {
        this.SKillView.SetActive(false);
        this.MySelf.SetActive(true);
        this.parse = TestParse.SkillSelectLearn;
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillId);
        if (skill != null)
        {
            this.m_Iamge_SkillIcon.sprite = WWWResourceManager.Instance.LoadSpriteFormAtla("common1.ab", skill.skillConfig.skillName);
            this.m_Text_SkillInfo.text = skill.skillConfig.skillInfo;
            this.m_Text_SkillName.text = skill.skillConfig.skillName;
            this.SkillLearn.SetActive(true);
        }
        if (!GuideModel.singleton.bIsGuideAllComp && bSelect == false)
        {
            GuideModel.singleton.NowTaskId = 6004;
            EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
        }
        else if (GuideModel.singleton.bIsGuideAllComp)
        {
            this.m_Button_Back.gameObject.SetActive(true);
        }
    }
    public void GuideToEnterLearn()
    {
        this.EnterLearn(this.selectSkillId);
    }
    public void EnterLock(int skillId)
    {
        if (!GuideModel.singleton.bIsGuideAllComp && GuideModel.singleton.bIsFirstLocked == false)
        {
            if (skillId > 0)
            {
                if (SkillManager.singleton.LockSkill(skillId))
                {
                    this.RefreshSkillItem(skillId);
                    GuideModel.singleton.bIsFirstLocked = true;
                }
            }
        }
        else
        {
            if (UnityMonoDriver.s_instance.ReleaseMode == false)
            {
                if (skillId > 0)
                {
                    if (SkillManager.singleton.LockSkill(skillId))
                    {
                        this.RefreshSkillItem(skillId);
                        //UserPrefsBase.singleton.AddMoney(-SkillManager.singleton.GetSkill(skillId).skillConfig.lockGold);
                        EventDispatch.Broadcast(Events.DlgFlyTextShow);
                        //提示解锁成功
                        EventDispatch.Broadcast<string>(Events.DlgAddSingleSystemInfo, "解锁成功!快去练习下吧！");
                    }
                }
                return;
            }
            //需要金币解锁
            if (UserPrefsBase.singleton.Money < SkillManager.singleton.GetSkill(skillId).skillConfig.lockGold)
            {
                EventDispatch.Broadcast(Events.DlgFlyTextShow);
                //提示金币不足
                EventDispatch.Broadcast<string>(Events.DlgAddSingleSystemInfo, "金币不足,请获取金币再来解锁!");
            }
            else
            {
                if (skillId > 0)
                {
                    if (SkillManager.singleton.LockSkill(skillId))
                    {
                        this.RefreshSkillItem(skillId);
                        UserPrefsBase.singleton.AddMoney(-SkillManager.singleton.GetSkill(skillId).skillConfig.lockGold);
                        EventDispatch.Broadcast(Events.DlgFlyTextShow);
                        //提示解锁成功
                        EventDispatch.Broadcast<string>(Events.DlgAddSingleSystemInfo, "解锁成功!快去练习下吧！");
                    }
                }
            }
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
    public void OnClickButtonSpeakTest()
    {
        GameSkillBase skill = SkillManager.singleton.GetSkill(this.selectSkillId);
        //等音频播放结束后,然后角色播放动作
        if (skill != null)
        {
            this.ShowSpeakButton(false); 
            //出现口令
            this.ShowToken(skill.skillConfig.skillToken);
            skill.Enter(this.entity,true);
            TimerManager.AddTimer(4000, 0, () => 
            {
                this.ShowToken("", false);
                this.ShowSpeakButton(true);
                if (!GuideModel.singleton.bIsGuideAllComp && GuideModel.singleton.bIsSelfSpeaked == false)
                {
                    //然后开始guide进入自己说
                    GuideModel.singleton.NowTaskId = 6005;
                    GuideModel.singleton.bIsSelfSpeaked = true;
                    EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
                }
            });
        }
    }
    public void OnClickButtonSelfSpeak()
    {
        this.parse = TestParse.SkillSpeakLearn;
        this.ButtonSpeak.SetActive(true);
        this.m_Button_Back.gameObject.SetActive(true);
        this.ShowSpeakButton(false);
    }
    public void OnClickButtonClose()
    {
        ClientGameStateManager.singleton.EnterDefaultState();
    }
    public void ShowMask(bool value)
    {
        if (this.SpeakMask.activeSelf != value)
        {
            this.SpeakMask.SetActive(value);
        }
    }
    private void ShowSpeakButton(bool value)
    {
        this.m_Button_Learn.gameObject.SetActive(value);
        this.m_Button_Test.gameObject.SetActive(value);
    }
    private void ShowToken(string token,bool Visible = true)
    {
        if (Visible)
        {
            this.m_Text_Token.text = token;
            this.SpeakToken.SetActive(true);
        }
        else
        {
            this.SpeakToken.SetActive(false);
        }
    }
    private void RefreshSkillItem(int itemId)
    {
        XUIListItem item = this.m_List_Skills.GetItemById(itemId);
        GameSkillBase skill = SkillManager.singleton.GetSkill(itemId);
        if (item != null && skill != null)
        {
            bool hasLocked = skill.bLocked;
            item.GetChild("sp_lock").gameObject.SetActive(!hasLocked);
            item.toggle.interactable = hasLocked;
            string btName = hasLocked ? "学习" : "解锁";
            item.SetText("bt_click/Text", btName);
        }
    }
    public void CastSkill(string token)
    {
        int skillId = -1;
        if (string.IsNullOrEmpty(token))
        {
            if (!GuideModel.singleton.bIsGuideAllComp)
            {
                //再次尝试
                GuideModel.singleton.NowTaskId = 6007;
                EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
            }
        }
        else
        {
            //出现口令
            skillId = SkillManager.singleton.MatchSkill(token);
            if (skillId == -1)
            {
                Debug.Log("匹配不到技能");
                this.ShowToken(token);
                if (!GuideModel.singleton.bIsGuideAllComp)
                {
                    //再次尝试
                    GuideModel.singleton.NowTaskId = 6007;
                    EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
                }
            }
        }
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillId);
        if (skill != null)
        {
            this.ShowToken(skill.skillConfig.skillName);
            skill.Enter(this.entity,true);
            if (!GuideModel.singleton.bIsGuideAllComp)
            {
                TimerManager.AddTimer(4000, 0, () =>
                {
                    GuideModel.singleton.NowTaskId = 6008;
                    EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
                });
            }        
        }
        if (this.ShowTokenTimerId != uint.MinValue)
        {
            TimerManager.DelTimer(this.ShowTokenTimerId);
            this.ShowTokenTimerId = TimerManager.AddTimer(4000, 0, () =>
            {
                this.ShowToken("", false);
            });
        }
        else
        {
            this.ShowTokenTimerId = TimerManager.AddTimer(4000, 0, () =>
            {
                this.ShowToken("", false);
            });
        }
    }
    public void CastSkillError(string error)
    {
        //防止最开始出现token气泡
        if (!GuideModel.singleton.bIsGuideAllComp && GuideModel.singleton.bIsSelfSpeaked == false)
        {
            return;
        }
        if (!GuideModel.singleton.bIsGuideAllComp)
        {
            //再次尝试
            GuideModel.singleton.NowTaskId = 6007;
            EventDispatch.Broadcast(Events.DlgGuideExecuteNextTask);
        }
        //出现口令
        this.ShowToken(error);
        this.ShowMask(false);
        if (this.ShowTokenTimerId != uint.MinValue)
        {
            TimerManager.DelTimer(this.ShowTokenTimerId);
            this.ShowTokenTimerId = TimerManager.AddTimer(4000, 0, () =>
            {
                this.ShowToken("", false);
            });
        }
        else
        {
            this.ShowTokenTimerId = TimerManager.AddTimer(4000, 0, () =>
            {
                this.ShowToken("", false);
            });
        }
    }
    public void OnClickButtonBack()
    {
        if (this.parse == TestParse.SkillSpeakLearn)
        {
            this.ButtonSpeak.SetActive(false);
            this.ShowSpeakButton(true);
            this.parse = TestParse.SkillSelectLearn;
        }
        else if (this.parse == TestParse.SkillSelectLearn)
        {
            this.SKillView.SetActive(true);
            this.MySelf.SetActive(false);
            this.SkillLearn.SetActive(false);
            this.selectSkillId = int.MinValue;
            this.m_List_Skills.UnSelectAllItems(false);
            this.m_Button_Back.gameObject.SetActive(false);
            this.parse = TestParse.SkillView;
        }
    }
}
public enum TestParse
{
    SkillView,
    SkillSelectLearn,
    SkillSpeakLearn
}
