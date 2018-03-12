using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CaomaoFramework.GameState
{
    public class ClientGameStateMachine
    {
        public Dictionary<string, ClientStateBase> m_dicClientStates = new Dictionary<string, ClientStateBase>();
        private ClientStateBase m_oCurrentClientState = null;
        private bool m_bScenePrepared = false;
        private bool m_bResourceLoaded = false;
        private ELoadingStyle m_eCurrentLoadingStyle = ELoadingStyle.DefaultRule;
        private Action m_aCallBackWhenChangeFinished = null;
        #region 属性
        public string CurrentGameState
        {
            get;
            private set;
        }
        public string NextGameState
        {
            get;
            private set;
        }
        public bool IsInChanging
        {
            get;
            private set;
        }
        #endregion
        public void Init()
        {
            this.m_dicClientStates.Add("StartState", new StartState());
            this.m_dicClientStates.Add("GameMainState", new GameMainState());
            this.CurrentGameState = "Max";
            this.IsInChanging = false;           
        }

        public void ConvertToState(string nextGameState, ELoadingStyle loadingStyle, Action callBackOnChangeFinished,string specialStateLoad = "BattleMainState")
        {
            try
            {
                if (!nextGameState.Equals(this.CurrentGameState))
                {
                    this.NextGameState = nextGameState;
                    this.m_eCurrentLoadingStyle = loadingStyle;
                    this.m_aCallBackWhenChangeFinished = (Action)Delegate.Combine(this.m_aCallBackWhenChangeFinished, callBackOnChangeFinished);
                    if ("Max" != this.CurrentGameState)
                    {
                        if (ELoadingStyle.DefaultRule == this.m_eCurrentLoadingStyle)
                        {
                            if (specialStateLoad == this.CurrentGameState)
                            {
                                this.m_eCurrentLoadingStyle = ELoadingStyle.LoadingNormal;
                            }
                            else
                            {
                                this.m_eCurrentLoadingStyle = ELoadingStyle.LoadingWait;
                            }
                        }
                        this.SetLoadingVisible(this.m_eCurrentLoadingStyle, true);
                    }
                    this.IsInChanging = true;
                    if ("Max" != this.CurrentGameState)
                    {
                        this.m_oCurrentClientState.OnLeave();
                        WWWResourceManager.Instance.SetAllUnLoadFinishedEventHandler(delegate (bool o)
                        {
                            this.DoChangeToNewState();
                        });
                    }
                    else
                    {
                        this.DoChangeToNewState();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        public void RegisterCallBackOnChangedFinished(Action callBackOnChangeFinished)
        {
            this.m_aCallBackWhenChangeFinished = (Action)Delegate.Combine(this.m_aCallBackWhenChangeFinished, callBackOnChangeFinished);
        }

        private void DoChangeToNewState()
        {
            this.CurrentGameState = this.NextGameState;
            this.NextGameState = "Max";
            this.m_bResourceLoaded = false;
            this.m_bScenePrepared = true;
            if (this.CurrentGameState == "GameMainState")
            {
                this.m_bScenePrepared = false;
                SceneManager.singleton.OnScenePerparedAction += () => 
                {
                    this.m_bScenePrepared = true;
                    this.OnPartLoaded();
                };
            }
            this.m_oCurrentClientState = this.m_dicClientStates[this.CurrentGameState];
            this.m_oCurrentClientState.OnEnter();
            WWWResourceManager.Instance.SetAllLoadFinishedEventHandler(delegate (bool o)
            {
                this.m_bResourceLoaded = true;
                this.OnPartLoaded();
            });
        }
        private void OnPartLoaded()
        {
            if (this.m_bResourceLoaded && this.m_bScenePrepared)
            {
                this.IsInChanging = false;
                this.SetLoadingVisible(this.m_eCurrentLoadingStyle, false);
                if (null != this.m_aCallBackWhenChangeFinished)
                {
                    Action callBackWhenChangeFinished = this.m_aCallBackWhenChangeFinished;
                    this.m_aCallBackWhenChangeFinished = null;
                    callBackWhenChangeFinished();
                }
            }
        }
        private void SetLoadingVisible(ELoadingStyle dlgType, bool bVisible)
        {
            switch (dlgType)
            {
                case ELoadingStyle.LoadingWait:
                    if (bVisible)
                    {
                        EventDispatch.Broadcast(EventsBase.OnLoadingWaitUIShow);
                    }
                    else
                    {
                        EventDispatch.Broadcast(EventsBase.OnLoadingWaitUIHide);
                    }
                    break;
                case ELoadingStyle.LoadingNormal:
                    if (bVisible)
                    {
                        EventDispatch.Broadcast(Events.DlgLoadingShow);
                    }
                    else
                    {
                        EventDispatch.Broadcast(Events.DlgLoadingHide);
                    }
                    break;
            }
        }      
    }
}
