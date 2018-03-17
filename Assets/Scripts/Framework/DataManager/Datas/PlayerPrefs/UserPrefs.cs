using System;
using System.Xml;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
namespace CaomaoFramework
{
    public class UserPrefsBase
    {
        private bool m_bHasSet = false;
        private bool m_bMuteSound = false;
        private bool m_bEnableMusic = true;
        private bool m_bEnableEffect = true;
        private float m_fBGSoundValue = 1f;
        private float m_fSoundValue = 1f;
        private float m_fVoiceValue = 1f;
        public List<int> guideFinishedId = new List<int>();
        private GraphicsQuality m_eGraphicsQuality = GraphicsQuality.Medium;
        private static UserPrefsBase s_instance = null;
        private IXLog m_log = XLog.GetLog<UserPrefsBase>();
        public static UserPrefsBase Singleton
        {
            get
            {
                if (UserPrefsBase.s_instance == null)
                {
                    UserPrefsBase.s_instance = new UserPrefsBase();
                    UserPrefsBase.s_instance.Init();
                }
                return UserPrefsBase.s_instance;
            }
        }
        public bool HasSet
        {
            get
            {
                return this.m_bHasSet;
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
        public UserPrefsBase()
        {
               
        }
        public virtual void Init()
        {
            try
            {
                this.LoadUserConfig();
            }
            catch (Exception ex)
            {
                this.m_log.Fatal(ex.ToString());
            }
        }
        public virtual void SaveUserConfig()
        {
            string fullPath = Application.dataPath + "/Resources/Config/UserConfig.xml";
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(newChild);
            XmlElement xmlElement = xmlDocument.CreateElement("root");
            xmlDocument.AppendChild(xmlElement);
            XmlElement xmlElement7 = xmlDocument.CreateElement("element");
            xmlElement7.SetAttribute("id", "mutesound");
            xmlElement7.SetAttribute("value", this.m_bMuteSound ? 1.ToString() : 0.ToString());
            xmlElement.AppendChild(xmlElement7);
            XmlElement xmlElement8 = xmlDocument.CreateElement("element");
            xmlElement8.SetAttribute("id", "bgsoundvalue");
            xmlElement8.SetAttribute("value", this.m_fBGSoundValue.ToString("f2"));
            xmlElement.AppendChild(xmlElement8);
            XmlElement xmlElement10 = xmlDocument.CreateElement("element");
            xmlElement10.SetAttribute("id", "soundvalue");
            xmlElement10.SetAttribute("value", this.m_fSoundValue.ToString("f2"));
            xmlElement.AppendChild(xmlElement10);
            XmlElement xmlElement11 = xmlDocument.CreateElement("element");
            xmlElement11.SetAttribute("id", "voicevalue");
            xmlElement11.SetAttribute("value", this.m_fVoiceValue.ToString("f2"));
            xmlElement.AppendChild(xmlElement11);
            XmlElement xmlElement12 = xmlDocument.CreateElement("element");
            xmlElement12.SetAttribute("id", "qualitysetting");
            xmlElement12.SetAttribute("value", string.Format("{0}", (int)this.m_eGraphicsQuality));
            xmlElement.AppendChild(xmlElement12);
            XmlElement xmlElement13 = xmlDocument.CreateElement("element");
            xmlElement13.SetAttribute("id", "enablemusic");
            xmlElement13.SetAttribute("value", this.m_bEnableMusic ? 1.ToString() : 0.ToString());
            xmlElement.AppendChild(xmlElement13);
            XmlElement xmlElement14 = xmlDocument.CreateElement("element");
            xmlElement14.SetAttribute("id", "enableeffect");
            xmlElement14.SetAttribute("value", this.m_bEnableEffect ? 1.ToString() : 0.ToString());
            xmlElement.AppendChild(xmlElement14);
            XmlElement xmlElement15 = xmlDocument.CreateElement("element");         
            if (GuideModel.singleton.GuideFinishedList.Count > 0)
            {
                string content = "";
                foreach (var guide in GuideModel.singleton.GuideFinishedList)
                {
                    content += guide + ",";
                }
                content.Remove(content.LastIndexOf(","));
                xmlElement14.SetAttribute("id", "guidefinish");
                xmlElement14.SetAttribute("value", content);
                xmlElement.AppendChild(xmlElement14);
            }       
            xmlDocument.Save(fullPath);
        }
        public virtual void LoadUserConfig()
        {
            string fullPath = Application.dataPath + "/Resources/Config/UserConfig.xml";
            if (File.Exists(fullPath))
            {
                using (XmlReader xmlReader = XmlReader.Create(fullPath))
                {
                    if (null != xmlReader)
                    {
                        this.m_bHasSet = true;
                        while (xmlReader.Read())
                        {
                            if (xmlReader.Name == "element" && xmlReader.NodeType == XmlNodeType.Element)
                            {
                                string text = xmlReader.GetAttribute("id").ToLower();
                                string attribute = xmlReader.GetAttribute("value");
                                string text2 = text;
                                switch (text2)
                                {
                                    case "mutesound":
                                        this.m_bMuteSound = (Convert.ToInt32(attribute) != 0);
                                        break;
                                    case "bgsoundvalue":
                                        this.m_fBGSoundValue = (float)Convert.ToDouble(attribute);
                                        break;
                                    case "soundvalue":
                                        this.m_fSoundValue = (float)Convert.ToDouble(attribute);
                                        break;
                                    case "voicevalue":
                                        this.m_fVoiceValue = (float)Convert.ToDouble(attribute);
                                        break;
                                    case "qualitysetting":
                                        this.m_eGraphicsQuality = (GraphicsQuality)Convert.ToInt32(attribute);
                                        break;
                                    case "enablemusic":
                                        this.m_bEnableMusic = (Convert.ToInt32(attribute) != 0);
                                        break;
                                    case "enableeffect":
                                        this.m_bEnableEffect = (Convert.ToInt32(attribute) != 0);
                                        break;
                                    case "guidefinish":
                                        if (attribute.Equals("null"))
                                        {
                                            GuideModel.singleton.bIsGuideAllComp = false;
                                            break;
                                        }
                                        string[] content = attribute.Split(',');
                                        foreach (var id in content)
                                        {
                                            this.guideFinishedId.Add(Convert.ToInt32(id));
                                        }
                                        break;                             
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
