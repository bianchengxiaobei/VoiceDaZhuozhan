﻿using System;
using System.Collections.Generic;
using CaomaoFramework;
using UnityEngine.UI;
using UnityEngine;
public class DlgMain : UIBase
{
    public GameObject SpeechMask;
    public XUIList m_List_SkillItems;
    public Text m_Text_WaveInfo;
    public Text m_Text_LevelName;
    public List<SkillView> skills = new List<SkillView>();
    public DlgMain()
    {
        this.mResName = "Assets.Prefabs.Guis.DlgMain.prefab";
        this.mResident = false;
        this.mELayer = EUILayer.Middle;
    }
    public override void Init()
    {
        EventDispatch.AddListener(Events.DlgMainShow, this.Show);
        EventDispatch.AddListener(Events.DlgMainHide, this.Hide);
    }

    public override void OnDisable()
    {
       
    }

    public override void OnEnable()
    {
        this.ShowMask(false);
        this.InitSkills();
        this.UpdateWaveInfo();
    }

    public override void Realse()
    {
       
    }

    protected override void InitWidget()
    {
        this.SpeechMask = this.mRoot.Find("sp_bg").gameObject;
        this.m_List_SkillItems = this.mRoot.Find("SkillView").GetComponent<XUIList>();
        this.m_Text_WaveInfo = this.mRoot.Find("lb_waveInfo").GetComponent<Text>();
        this.m_Text_LevelName = this.mRoot.Find("lb_levelName").GetComponent<Text>();
    }

    protected override void OnAddListener()
    {
        EventDispatch.AddListener<bool>(Events.DlgMainShowMask, this.ShowMask);
        EventDispatch.AddListener<bool>(Events.DlgMainEndLevel, this.EndLevel);
        EventDispatch.AddListener(Events.DlgMainRefresh, this.UpdateWaveInfo);
    }

    protected override void OnRemoveListener()
    {
        EventDispatch.RemoveListener<bool>(Events.DlgMainShowMask, this.ShowMask);
        EventDispatch.RemoveListener<bool>(Events.DlgMainEndLevel, this.EndLevel);
    }

    protected override void RealseWidget()
    {
       
    }
    public void ShowMask(bool value)
    {
        this.SpeechMask.SetActive(value);
    }
    public void InitSkills()
    {
        int index = 0;
        foreach (var skill in SkillManager.singleton.lockedSkills)
        {
            XUIListItem skillItem = null;
            if (index < this.m_List_SkillItems.Count)
            {
                skillItem = this.m_List_SkillItems.GetItemByIndex(index);
            }
            else
            {
                skillItem = this.m_List_SkillItems.AddListItem();
            }
            if (skillItem != null)
            {
                skillItem.Id = skill.skillConfig.skillId;
                SkillView skillView = skillItem.GetComponent<SkillView>();
                skillView.skillIcon.sprite = WWWResourceManager.Instance.LoadSpriteFormAtla("common1.ab", skill.skillConfig.iconPath);
                skillView.SetSkillNameAndId(skill.skillConfig.skillId);
                this.skills.Add(skillView);
            }
            index++;
        }
    }
    public void UpdateWaveInfo()
    {
        string info = string.Format("当前波数 : {0}/{1}", LevelManager.singleton.curWaveIndex+1, LevelManager.singleton.GetCurWaveNum());
        this.m_Text_WaveInfo.text = info;
        this.m_Text_LevelName.text = LevelManager.singleton.GetCurLevel().levelName;
    }
    public void EndLevel(bool bWin)
    {
        if (bWin)
        {
            //开始解锁下一关
            LevelManager.singleton.LockedNextLevel();
            //增加金币
            UserPrefsBase.singleton.AddMoney(50);
            //播放胜利音乐
            AudioManagerBase.Instance.PlayMusic(CommonDefineBase.VictoryAudioPath);
            //显示胜利界面
            TimerManager.AddTimer(2000,0,()=>
            {
                SkillManager.singleton.ClearSkill();
                EventDispatch.Broadcast(Events.DlgFlyTextShow);
                EventDispatch.Broadcast<string, Action, Action>(Events.DlgAddDoubleSystemInfo, "恭喜通过本关！是否继续开始下一关?",
                () =>
                {
                    //停止播放胜利音乐
                    AudioManagerBase.Instance.StopMusic();
                    AudioManagerBase.Instance.PlayMusic("Assets.Audios.Musics.Main.mp3");
                    LevelManager.singleton.EnterLevel(LevelManager.singleton.currLevelId);
                    PlayerManager.singleton.CreateAllEntity();
                    VoiceManager.Instance.RegisterCallback(PlayerManager.singleton.MySelf.CastSkill);
                    //刷新界面
                    EventDispatch.Broadcast(Events.DlgMainRefresh);
                    //加载所有怪物
                    PlayerManager.singleton.LoadAllEntity(() =>
                    {
                        PlayerManager.singleton.ShowMySelf();
                        PlayerManager.singleton.ShowCurWaveMaster();
                    });
                },
                () =>
                {
                    ClientGameStateManager.singleton.ChangeGameState("StartState");
                });
            });
        }
        else
        {
            //播放失败音乐
            AudioManagerBase.Instance.PlayMusic(CommonDefineBase.DefaultAudioPath);
            //显示失败界面
            TimerManager.AddTimer(2000, 0, () =>
            {
                SkillManager.singleton.ClearSkill();
                EventDispatch.Broadcast(Events.DlgFlyTextShow);
                EventDispatch.Broadcast<string, Action>(Events.DlgAddSingleActionSystemInfo, "挑战失败！是否重新开始?(小提示：可以去练功房学习更多的技能再来挑战！)",
                () =>
                {
                    ClientGameStateManager.singleton.ChangeGameState("StartState");
                });
            });
        }
    }
}

