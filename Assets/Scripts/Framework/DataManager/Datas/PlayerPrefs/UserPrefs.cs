using System;
using System.Xml;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    public class UserPrefsBase : Singleton<UserPrefsBase>
    {
        private bool m_bHasSet = false;
        private bool m_bMuteSound = false;
        private bool m_bEnableMusic = true;
        private bool m_bEnableEffect = true;
        private float m_fBGSoundValue = 1f;
        private float m_fSoundValue = 1f;
        private float m_fVoiceValue = 1f;
        private int money = 0;
        public List<int> guideFinishedId = new List<int>();
        private GraphicsQuality m_eGraphicsQuality = GraphicsQuality.Medium;
        public int Money
        {
            get
            {
                return this.money;
            }
        }
        public bool IsMute
        {
            get
            {
                return this.m_bMuteSound;
            }
            set
            {
                this.m_bMuteSound = value;
            }
        }
        public float BGSoundValue
        {
            get
            {
                return this.m_fBGSoundValue;
            }
            set
            {
                this.m_fBGSoundValue = value;
            }
        }
        public float SoundValue
        {
            get
            {
                return this.m_fSoundValue;
            }
            set
            {
                this.m_fSoundValue = value;
            }
        }
        public float VoiceValue
        {
            get
            {
                return this.m_fVoiceValue;
            }
            set
            {
                this.m_fVoiceValue = value;
            }
        }
        public GraphicsQuality Quality
        {
            get
            {
                return this.m_eGraphicsQuality;
            }
            set
            {
                this.m_eGraphicsQuality = value;
            }
        }
        public bool EnableMusic
        {
            get { return this.m_bEnableMusic; }
            set { this.m_bEnableMusic = value; }
        }
        public bool EnableEffect
        {
            get { return this.m_bEnableEffect; }
            set { this.m_bEnableEffect = value; }
        }
        public void SaveUserConfig()
        {
            if (this.m_bHasSet == false)
                return;
            PlayerPrefs.SetInt("mutesound", this.IsMute ? 1 : 0);
            PlayerPrefs.SetFloat("bgsoundvalue", this.BGSoundValue);
            PlayerPrefs.SetFloat("voicevalue", this.VoiceValue);
            PlayerPrefs.SetFloat("soundvalue", this.SoundValue);
            PlayerPrefs.SetInt("enablemusic", this.EnableMusic ? 1 : 0);
            PlayerPrefs.SetInt("enableeffect", this.EnableEffect ? 1 : 0);
            PlayerPrefs.SetInt(CommonDefineBase.Money, this.money);
            PlayerPrefs.Save();
        }
        public void LoadUserConfig()
        {
            if (!PlayerPrefs.HasKey("mutesound"))
            {
                PlayerPrefs.SetInt("mutesound", 0);
            }
            else
            {
                this.m_bMuteSound = PlayerPrefs.GetInt("mutesound") != 0;
            }
            if (!PlayerPrefs.HasKey("bgsoundvalue"))
            {
                PlayerPrefs.SetFloat("bgsoundvalue", 1);
            }
            else
            {
                this.m_fBGSoundValue = PlayerPrefs.GetFloat("bgsoundvalue");
            }
            if (!PlayerPrefs.HasKey("soundvalue"))
            {
                PlayerPrefs.SetFloat("soundvalue", 1);
            }
            else
            {
                this.m_fSoundValue = PlayerPrefs.GetFloat("soundvalue");
            }
            if (!PlayerPrefs.HasKey("voicevalue"))
            {
                PlayerPrefs.SetFloat("voicevalue", 1);
            }
            else
            {
                this.m_fSoundValue = PlayerPrefs.GetFloat("voicevalue");
            }
            if (!PlayerPrefs.HasKey("enablemusic"))
            {
                PlayerPrefs.SetInt("enablemusic", 1);
            }
            else
            {
                this.m_bEnableMusic = PlayerPrefs.GetInt("enablemusic") != 0;
            }
            if (!PlayerPrefs.HasKey("enableeffect"))
            {
                PlayerPrefs.SetInt("enableeffect", 1);
            }
            else
            {
                this.m_bEnableEffect = PlayerPrefs.GetInt("enableeffect") != 0;
            }
            if (!PlayerPrefs.HasKey(CommonDefineBase.Money))
            {
                this.money = 0;
                PlayerPrefs.SetInt(CommonDefineBase.Money, this.money);
            }
            else
            {
                this.money = PlayerPrefs.GetInt(CommonDefineBase.Money);
            }
            if (!PlayerPrefs.HasKey(CommonDefineBase.GuideFinish))
            {
                GuideModel.singleton.bIsGuideAllComp = false;
            }
            else
            {
                if (PlayerPrefs.GetString(CommonDefineBase.GuideFinish).Contains("ok"))
                {
                    GuideModel.singleton.bIsGuideAllComp = true;
                    return;
                }
                string[] content = PlayerPrefs.GetString(CommonDefineBase.GuideFinish).Split(',');
                foreach (var id in content)
                {
                    this.guideFinishedId.Add(Convert.ToInt32(id));
                }
            }
            this.m_bHasSet = true;
        }
        public void AddMoney(int money)
        {
            this.money += money;
            //刷新主界面
            EventDispatch.Broadcast(Events.DlgStartMoneyUpdate);
        }
    }
}
