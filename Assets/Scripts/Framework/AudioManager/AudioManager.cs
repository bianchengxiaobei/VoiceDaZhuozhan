using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Resource;
using CaomaoFramework.Audio;
using Tangzx.ABSystem;
namespace CaomaoFramework
{
    public class AudioManagerBase : IAudioManager
    {
        #region 字段
        private string m_strCurMusicFile = string.Empty;
        private AudioSource m_curMusicAudioSource = null;
        private AudioSource m_curUiEffectAudioSource = null;
        private AudioSource m_curReportEffectAudioSource = null;
        private AssetBundleInfo m_assetRequestMusic = null;
        private float m_fVolumeBgMusic = 1f;
        private float m_fVolumeEffect = 1f;
        private float m_fVolumeVoice = 1f;
        private bool m_bIsMuted = false;
        private bool m_bEnableMusic = true;
        private bool m_bEnableEffect = true;
        private bool m_bEnable = true;
        private bool m_bNeedFadeOut = false;
        private float m_fFadeOutDuration = 1f;
        private float m_fFadeOutStartTime = 0f;
        private GameObject m_gameObjectCached = null;
        private static AudioManagerBase m_oInstace;
        #endregion
        #region 属性
        /// <summary>
        /// 背景音乐的音量
        /// </summary>
        public float VolumeBgMusic
        {
            get { return this.m_fVolumeBgMusic; }
            set
            {
                this.m_fVolumeBgMusic = value;
            }
        }
        /// <summary>
        /// 特效的音量
        /// </summary>
        public float VolumeEffect
        {
            get { return this.m_fVolumeEffect; }
            set { this.m_fVolumeEffect = value; }
        }
        /// <summary>
        /// 人物的音量
        /// </summary>
        public float VolumeVoice
        {
            get { return this.m_fVolumeVoice; }
            set
            {
                this.m_fVolumeVoice = value;
                UserPrefsBase.Singleton.VoiceValue = this.m_fVolumeVoice;
            }
        }
        /// <summary>
        /// 是否是静音
        /// </summary>
        public bool IsMuted
        {
            get { return this.m_bIsMuted; }
            set { this.m_bIsMuted = value; }
        }
        /// <summary>
        /// 是否启用背景音乐
        /// </summary>
        public bool EnableMusic
        {
            get { return this.m_bEnableMusic; }
            set
            {
                this.m_bEnableMusic = value;
                UserPrefsBase.Singleton.EnableMusic = value;
                if (this.m_curMusicAudioSource != null)
                {
                    this.m_curMusicAudioSource.enabled = value;
                }
            }
        }
        /// <summary>
        /// 是否启用特效音效
        /// </summary>
        public bool EnableEffect
        {
            get { return this.m_bEnableEffect; }
            set
            {
                this.m_bEnableEffect = value;
                UserPrefsBase.Singleton.EnableEffect = value;
            }
        }
        public static AudioManagerBase Instance
        {
            get
            {
                if (m_oInstace == null)
                {
                    m_oInstace = new AudioManagerBase();
                }
                return m_oInstace;
            }
        }
        #endregion
        #region 构造方法
        #endregion
        #region 公有方法
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            this.m_gameObjectCached = new GameObject("AudioManager");
            GameObject.DontDestroyOnLoad(this.m_gameObjectCached);
            this.m_curMusicAudioSource = this.m_gameObjectCached.AddComponent<AudioSource>();
            this.m_curUiEffectAudioSource = this.m_gameObjectCached.AddComponent<AudioSource>();
            this.m_curReportEffectAudioSource = this.m_gameObjectCached.AddComponent<AudioSource>();
            this.IsMuted = UserPrefsBase.Singleton.IsMute;
            this.VolumeBgMusic = UserPrefsBase.Singleton.BGSoundValue;
            this.VolumeEffect = UserPrefsBase.Singleton.SoundValue;
            AddListeners();
        }
        public void AddListeners()
        {
            
        }
        public void RemoveListeners()
        {
            
        }
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="strAudio"></param>
        public void PlayMusic(string strAudio)
        {
            if (null == this.m_curMusicAudioSource)
            {
                Debug.LogError("null == AudioSource");
            }
            else
            {
                this.m_bNeedFadeOut = false;
                if (strAudio != this.m_strCurMusicFile)
                {
                    WWWResourceManager.Instance.Load(strAudio, this.OnLoadMusicFinished);
                    this.m_strCurMusicFile = strAudio;
                }
            }
        }
        /// <summary>
        /// 停止播放音乐
        /// </summary>
        public void StopMusic()
        {
            if (null == this.m_curMusicAudioSource)
            {
               Debug.LogError("null == AudioSource");
            }
            else
            {
                this.m_curMusicAudioSource.Stop();
                this.m_curMusicAudioSource.clip = null;
                if (null != this.m_assetRequestMusic)
                {
                    this.m_assetRequestMusic.Release();
                    this.m_assetRequestMusic = null;
                }
            }
        }
        /// <summary>
        /// 播放UISound
        /// </summary>
        /// <param name="clip"></param>
        public void PlayUIEffectSound(AudioClip clip)
        {
            PlaySoundOnSourceByObject(this.m_curUiEffectAudioSource, clip);
        }
        /// <summary>
        /// 播放ReportSound
        /// </summary>
        /// <param name="clip"></param>
        public void PlayReportEffectSound(AudioClip clip)
        {
            PlaySoundOnSourceByObject(this.m_curReportEffectAudioSource, clip);
        }
        /// <summary>
        /// 淡出音乐（从有到无）
        /// </summary>
        /// <param name="duration">淡出间隔</param>
        public void FadeOutMusic(float duration)
        {
            this.m_bNeedFadeOut = true;
            this.m_fFadeOutDuration = duration > 0.1f ? duration : 0.1f;
            this.m_fFadeOutStartTime = Time.time;
        }
        public AudioOneShotPlay PlayAudioOneShot(string strAudioFile, Action<AudioOneShotPlay> callBack)
        {
            return this.PlayAudioOneShot(strAudioFile, this.VolumeEffect, callBack);
        }
        public AudioOneShotPlay PlayAudioOneShot(string strAudioFile, float fVolume, Action<AudioOneShotPlay> callBack)
        {
            AudioOneShotPlay ins = CInstacePoolKeepNumber<AudioOneShotPlay>.Alloc();
            ins.m_hostObject = this.m_gameObjectCached;
            ins.m_audioFile = strAudioFile;
            ins.m_audioVolume = fVolume;
            ins.m_callBack = callBack;
            ins.IsStopped = false;
            WWWResourceManager.Instance.Load(strAudioFile, ins.OnLoadOneShotAudioFinished);
            return ins;
        }
        /// <summary>
        /// 播放实体音效
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clip"></param>
        public void LogicPlaySoundByClip(AudioSource source, AudioClip clip)
        {
            PlaySoundOnSourceByObject(source, clip);
        }

        /// <summary>
        /// 如果有音乐淡出的话，就慢慢减小音量，然后停止
        /// </summary>
        public void Update()
        {
            if (this.m_bNeedFadeOut)
            {
                float num = (this.m_fFadeOutDuration + this.m_fFadeOutStartTime - Time.time) / this.m_fFadeOutDuration;
                if (num > 0f)
                {
                    if (this.m_curMusicAudioSource != null)
                    {
                        this.m_curMusicAudioSource.volume = num * this.m_fVolumeBgMusic;
                    }
                }
                else
                {
                    this.m_bNeedFadeOut = false;
                    this.StopMusic();
                }
            }
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 加载音频的回调函数，主要是处理播放
        /// </summary>
        /// <param name="assetRequest"></param>
        private void OnLoadMusicFinished(AssetBundleInfo assetRequest)
        {
            //如果不存在音频就停止播放
            if (null == assetRequest || null == assetRequest.data)
            {
                Debug.LogError("不存在音频");
                this.m_curMusicAudioSource.Stop();
                this.m_curMusicAudioSource.clip = null;
            }
            else
            {
                if (this.m_assetRequestMusic != null)
                {
                    this.m_assetRequestMusic.Dispose();
                }
                this.m_assetRequestMusic = assetRequest;
                this.m_curMusicAudioSource.clip = (assetRequest.mainObject as AudioClip);
                assetRequest.Retain();
                this.m_curMusicAudioSource.loop = true;
                this.m_curMusicAudioSource.volume = this.m_fVolumeBgMusic;
                if (this.m_bEnableMusic || this.m_bEnable)
                {
                    this.m_curMusicAudioSource.Play();
                }
            }
        }
        private void PlaySoundOnSourceByObject(AudioSource gameObjectAudioSource, AudioClip clip, bool isLoop = false)
        {
            if (null == clip || null == gameObjectAudioSource)
            {
                Debug.LogWarning("实体没有AudioSource,AudioClip不存在");
                return;
            }
            gameObjectAudioSource.clip = clip;
            gameObjectAudioSource.volume = this.VolumeEffect;
            gameObjectAudioSource.loop = isLoop;
            gameObjectAudioSource.Play();
        }
        #endregion
    }
}
