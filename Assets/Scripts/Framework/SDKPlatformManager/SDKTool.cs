using System;
using System.Collections.Generic;
using UnityEngine;
namespace CaomaoFramework.SDK
{
    public class SDKTool
    {
        private static SDKTool mInst = null;
        private ISDKTool m_sdkToolImpl = null;
        private Queue<SDKEvent> m_queueSDKEvent = new Queue<SDKEvent>();
        public static SDKTool Singleton
        {
            get
            {
                if (SDKTool.mInst == null)
                {
                    SDKTool.mInst = new SDKTool();
                }
                return SDKTool.mInst;
            }
        }
        public EPlatformType EPlatformType
        {
            get
            {
                if (null == this.m_sdkToolImpl)
                {
                    return EPlatformType.Platform_None;
                }
                else
                {
                    return this.m_sdkToolImpl.EPlatformType;
                }
            }
        }


        public bool Init(ISDKTool sdktool)
        {
            if (null == sdktool)
            {
                return false;
            }
            this.m_sdkToolImpl = sdktool;
            return this.m_sdkToolImpl.Init();
        }
        public void Install(string file)
        {
            if (this.m_sdkToolImpl != null)
            {
                this.m_sdkToolImpl.Install(file);
            }
        }
        public string GetIdFV()
        {
            string result = null;
            if (this.m_sdkToolImpl != null)
            {
                result = this.m_sdkToolImpl.GetIdFV();
                Debug.Log("IDFV:" + result);
            }
            return result;
        }
        public void Login()
        {
            if (this.m_sdkToolImpl != null)
            {
                this.m_sdkToolImpl.Login();
            }
        }
        public void CheckUpdate()
        {
            //UpdateManager.singleton.Init(this.OnFileCompress,this.OnProgress,this.OnTaskProgress
            //    ,this.OnDownloadError,this.OnError,this.OnFinish);
            //UpdateManager.singleton.CheckVersion();
        }
        private void OnFileCompress(bool finished)
        {
            //UnityMonoDriver.s_instance.Invoke(() => 
            //{
            //    if (finished)
            //    {
            //        DefaultUI.Instance.SetTipContent("正在更新本地文件");
            //    }
            //    else
            //    {
            //        DefaultUI.Instance.SetTipContent("数据读取中");
            //    }
            //});
        }
        private void OnProgress(float percentage, long totalRec, long received)
        {
            //UnityMonoDriver.s_instance.Invoke(() =>
            //{
            //    DefaultUI.Instance.SetProgress(percentage);
            //});
        }
        private void OnTaskProgress(int total, int index, string fileName)
        {
            //UnityMonoDriver.s_instance.Invoke(() =>
            //{
            //    DefaultUI.Instance.SetTipContent(string.Format("正在下载更新文件（{0}/{1}:{2}）", index + 1, total, fileName));
            //});
        }
        private void OnDownloadError(Exception e)
        {
            //让他重新check
            Debug.LogError("下载出错");
        }
        private void OnError()
        {
            Debug.LogError("下载出错22");
        }
        private void OnFinish()
        {
            //DefaultUI.Instance.Release();
            //DefaultUI.Instance.ShowProgressBar(false);
            //进入到登陆界面
            UnityMonoDriver.s_instance.LaterInit();
        }
        public bool PopEvent(out SDKEvent sdkEvent)
        {
            bool result;
            if (this.m_queueSDKEvent.Count > 0)
            {
                sdkEvent = this.m_queueSDKEvent.Dequeue();
                result = true;
            }
            else
            {
                sdkEvent = null;
                result = false;
            }
            return result;
        }
        public SDKEvent PushEvent(EnumSDKEventType eSdkEventType)
        {
            SDKEvent e = new SDKEvent();
            e.Type = eSdkEventType;
            this.m_queueSDKEvent.Enqueue(e);
            return e;
        }

    }
}
