using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework
{
    public class UIManager : Singleton<UIManager>
    {
        public Dictionary<string, UIBase> m_dicUIs = new Dictionary<string, UIBase>();
        public UIManager()
        {
            m_dicUIs.Add("DlgStart", new DlgStart());
            m_dicUIs.Add("DlgLevel", new DlgLevel());
            m_dicUIs.Add("DlgMain", new DlgMain());
            m_dicUIs.Add("DlgLoading", new DlgLoading());
            m_dicUIs.Add("DlgGuide", new DlgGuide());
            m_dicUIs.Add("DlgText", new DlgText());
            m_dicUIs.Add("DlgFlyText", new DlgFlyText());
        }

        public void Init()
        {
            foreach (var ui in this.m_dicUIs.Values)
            {
                ui.Init();
                if (ui.IsResident())
                {
                    ui.PreLoad();
                }
            }
        }
        public void Update(float deltaTime)
        {
            foreach (var ui in this.m_dicUIs)
            {
                if (ui.Value.IsVisible())
                {
                    ui.Value.Update(deltaTime);
                }

            }
        }
        public void OnGUI()
        {
            foreach (var ui in this.m_dicUIs.Values)
            {
                if (ui.IsVisible())
                {
                    ui.OnGUI();
                }
            }
        }
        public UIBase GetUI(string type)
        {
            if (m_dicUIs.ContainsKey(type))
            {
                return m_dicUIs[type];
            }
            else
            {
                return null;
            }
        }     
    }
}
