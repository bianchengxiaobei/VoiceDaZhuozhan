using System;
using UnityEngine;
namespace CaomaoFramework
{
    public class UnityMonoDriver : MonoBehaviour
    {
        public static UnityMonoDriver s_instance = null;

        public ClientGameStateManager clientGameStateManager = ClientGameStateManager.singleton;

        public SDKPlatformManager sdkManager = SDKPlatformManager.singleton;

        public UIManager uiManager = UIManager.singleton;

        public string debugInfo = "";
        public bool ReleaseMode = false;
        private void Awake()
        {
            Application.targetFrameRate = 30;
            Application.runInBackground = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            UnityMonoDriver.s_instance = this;
            if (base.transform.parent != null)
            {
                DontDestroyOnLoad(base.transform.parent);
            }
            InvokeRepeating("Tick",0f, 0.01f);
            //NetworkManager.singleton.Init();
            clientGameStateManager.m_oClientStateMachine.Init();
            sdkManager.Init();
            //AdsManager.singleton.Init();
        }
        public void LaterInit()
        {
            WWWResourceManager.Instance.Init(() =>
            {
                uiManager.Init();
                //Login.singleton.RequestAnnouncement();
                UserPrefsBase.Singleton.LoadUserConfig();
                LevelManager.singleton.LoadLevel();
                SkillManager.singleton.LoadSkill();
                GuideController.singleton.GuideFinishModelList(UserPrefsBase.Singleton.guideFinishedId);
                WWWResourceManager.Instance.SetAllLoadFinishedEventHandler((ok) =>
                {
                    clientGameStateManager.EnterDefaultState();
                });
            });
        }
        private void Start()
        {
            //Analyzer.singleton.Enable = true;
            AudioManagerBase.Instance.Init();
            LogoUI.Instance.PlayFadeStartLogo(() =>
            {
                sdkManager.Install();
            });
        }
        private void Update()
        {          
            sdkManager.Update();
            AudioManagerBase.Instance.Update();
            //LocalEffectManager.singleton.OnUpdate();
            //Analyzer.singleton.Update();
            uiManager.Update(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Escape))
                this.ApplicationQuit();
            //NetworkManager.singleton.FixedUpdate();
        }
        private void OnGUI()
        {
            //if (Analyzer.singleton.Enable)
            //{
            //    GUI.backgroundColor = Color.black;
            //    GUI.color = Color.red;
            //    GUI.contentColor = Color.red;
            //    GUI.Label(new Rect(100f, 100f, 100f, 100f), string.Format("FPS:{0}", Analyzer.singleton.FPS));
            //    GUI.Label(new Rect(200f, 100f, 100f, 100f), string.Format("Send:{0}", Analyzer.singleton.SendRate));
            //    GUI.Label(new Rect(300f, 100f, 100f, 100f), string.Format("Receive:{0}", Analyzer.singleton.ReceiveRate));
            //    GUI.Label(new Rect(400f, 100f, 100f, 100f), string.Format("Ping:{0}", Analyzer.singleton.Ping));
            //    //GUI.Label(new Rect(100f, 500f, 300f, 100f), string.Format("DebugInfo:{0}", this.debugInfo));
            //}
            //uiManager.OnGUI();
        }
        private void FixedUpdate()
        {         
            //ReConnect.singleton.FixedUpdate();
        }
        private void Tick()
        {
            TimerManager.Tick();
            FrameTimerManager.Tick();
        }
        public void Invoke(Action action)
        {
            TimerManager.AddTimer(0, 0, action);
        }
        private void ApplicationQuit()
        {
            //EventDispatch.Broadcast(Events.DlgFlyTextShow);
            //EventDispatch.Broadcast<string, Action, Action>(Events.DlgAddDoubleSystemInfo, "确定要退出游戏", () =>
            //{
            //    Application.Quit();
            //}, 
            //null);
        }
    }
}
