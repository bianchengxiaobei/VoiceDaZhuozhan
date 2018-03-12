﻿using System;
using System.Collections.Generic;
using CaomaoFramework.GameState;
using UnityEngine;
namespace CaomaoFramework
{
    /// <summary>
    /// 游戏状态管理器，提供改变游戏状态的接口
    /// </summary>
    [System.Serializable]
    public class ClientGameStateManager : Singleton<ClientGameStateManager>
    {
        private Queue<ClientStateChangeArgs> m_qClientStateQueue = new Queue<ClientStateChangeArgs>();
        public ClientGameStateMachine m_oClientStateMachine = new ClientGameStateMachine();
        public string m_sDefalutGameStateName ;
        /// <summary>
        /// 当前的状态
        /// </summary>
        public string CurrentClientState
        {
            get
            {
                return this.m_oClientStateMachine.CurrentGameState;
            }
        }
        /// <summary>
        /// 下个状态
        /// </summary>
        public string ENextGameState
        {
            get
            {
                return this.m_oClientStateMachine.NextGameState;
            }
        }
        /// <summary>
        /// 是否正在改变状态
        /// </summary>
        public bool IsInChangingState
        {
            get
            {
                return this.m_oClientStateMachine.IsInChanging;
            }
        }
        /// <summary>
        /// 改变游戏状态
        /// </summary>
        /// <param name="eGameState"></param>
        public void ChangeGameState(string eGameState)
        {
            this.ChangeGameState(eGameState, ELoadingStyle.DefaultRule);
        }
        /// <summary>
        /// 改变游戏状态
        /// </summary>
        /// <param name="eGameState"></param>
        /// <param name="loadingStyle"></param>
        public void ChangeGameState(string eGameState, ELoadingStyle loadingStyle)
        {
            this.ChangeGameState(eGameState, loadingStyle, null);
        }
        /// <summary>
        /// 改变游戏状态
        /// </summary>
        /// <param name="eGameState"></param>
        /// <param name="loadingStyle"></param>
        /// <param name="callBackWhenChangeFinished"></param>
        public void ChangeGameState(string eGameState, ELoadingStyle loadingStyle, Action callBackWhenChangeFinished)
        {
            if (this.IsInChangingState)
            {
                ClientStateChangeArgs item = new ClientStateChangeArgs
                {
                    sClientState = eGameState,
                    eLoadingStyle = loadingStyle,
                    aCallBack = callBackWhenChangeFinished
                };
                this.m_qClientStateQueue.Enqueue(item);
            }
            else
            {
                this.m_oClientStateMachine.ConvertToState(eGameState, loadingStyle, callBackWhenChangeFinished);
                this.RegisterCallBackOnChangedFinished(this.ChangeGameStateQueue);
            }
        }
        /// <summary>
        /// 进入到默认入口
        /// </summary>
        /// <param name="defaultStateName"></param>
        public void EnterDefaultState()
        {
            if (string.IsNullOrEmpty(this.m_sDefalutGameStateName))
            {
                return;
            }
            else
            {
                this.ChangeGameState(this.m_sDefalutGameStateName);
            }
        }
        public void ChangeGameStateQueue()
        {
            if (0 != this.m_qClientStateQueue.Count)
            {
                ClientStateChangeArgs stateChangeArgs = this.m_qClientStateQueue.Dequeue();
                this.ChangeGameState(stateChangeArgs.sClientState, stateChangeArgs.eLoadingStyle, stateChangeArgs.aCallBack);
            }
        }
        public void RegisterCallBackOnChangedFinished(Action callBackWhenChangeFinished)
        {
            this.m_oClientStateMachine.RegisterCallBackOnChangedFinished(callBackWhenChangeFinished);
        }
    }
}
