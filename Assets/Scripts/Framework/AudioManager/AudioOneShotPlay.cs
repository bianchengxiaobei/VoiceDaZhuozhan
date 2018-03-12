using System.Collections;
using UnityEngine;
using CaomaoFramework.Resource;
using Tangzx.ABSystem;
using CaomaoFramework;
using System;

namespace CaomaoFramework.Audio
{
    [AInstanceNumber(10)]
    public class AudioOneShotPlay : IInstancePoolObject
    {
        #region 字段
        public GameObject m_hostObject;
        public Action<AudioOneShotPlay> m_callBack;
        public AudioSource m_audioSource;
        public float m_audioVolume;
        public string m_audioFile;
        #endregion
        #region 属性
        public bool IsStopped
        {
            get;
            set;
        }
        #endregion
        #region 构造方法
        public AudioOneShotPlay()
        {

        }
        #endregion
        #region 公有方法
        public void Stop()
        {
            this.IsStopped = true;
            if (null != this.m_audioSource)
            {
                UnityEngine.Object.Destroy(this.m_audioSource);
            }
        }
        #endregion
        #region 私有方法
        /// <summary>
        /// 加载音效回调
        /// </summary>
        /// <param name="assetRequest"></param>
        public void OnLoadOneShotAudioFinished(AssetBundleInfo assetRequest)
        {
            if (assetRequest != null && assetRequest.mainObject != null)
            {
                if (!this.IsStopped)
                {
                    this.m_audioSource = this.m_hostObject.AddComponent<AudioSource>();
                    this.m_audioSource.clip = assetRequest.Require(this.m_audioSource) as AudioClip;
                    this.m_audioSource.volume = this.m_audioVolume;
                    this.m_audioSource.Play();
                    UnityMonoDriver.s_instance.StartCoroutine(this.AutoDistroyOneShotAudio());
                }
            }
        }
        /// <summary>
        /// 播放音效完成之后自动摧毁
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoDistroyOneShotAudio()
        {
            if (null == this.m_audioSource.clip)
            {
                Debug.LogError(this.m_audioFile + " does not exist");
                UnityEngine.Object.Destroy(this.m_audioSource);
                this.IsStopped = true;
            }
            else
            {
                if (this.m_audioSource.clip.length <= 0f)
                {
                    Debug.Log(this.m_audioSource.clip.name + " length is zero");
                }
                yield return new WaitForSeconds(this.m_audioSource.clip.length);//等待音效播放完成
                if (this.m_audioSource != null && !this.IsStopped)
                {
                    UnityEngine.Object.Destroy(this.m_audioSource);
                    this.IsStopped = true;
                    if (null != this.m_callBack)
                    {
                        this.m_callBack(this);//音效播放完成后回调
                    }
                }
            }
            yield break;
        }

        public void OnAlloc()
        {
            
        }

        public void OnRelease()
        {
            this.m_audioSource = null;
            this.m_hostObject = null;
            this.m_callBack = null;
            this.IsStopped = false;
        }
        #endregion
    }
}
