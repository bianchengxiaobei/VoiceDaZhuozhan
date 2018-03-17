using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework.SDK;
using CaomaoFramework;
using System.Net.NetworkInformation;
//using cn.sharesdk.unity3d;
using System.Collections;
/*----------------------------------------------------------------
// 模块名：AndroidSDKToolImpl
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class AndroidSDKToolImpl : ISDKTool
{
   // private ShareSDK shareSdk;
    public EPlatformType EPlatformType
    {
        get
        {
            return EPlatformType.Platform_Tencent;
        }
    }

    public string GetIdFV()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public bool Init()
    {
        ////shareSdk = UnityMonoDriver.s_instance.GetComponent<ShareSDK>();
        //if (shareSdk == null)
        //{
        //    Debug.LogError("ShareSdk == null");
        //    return false;
        //}
        return true;
    }

    public void Install(string file)
    {
        //DefaultUI.Instance.SetTipContent("正在检查网络...");
        PreInstall();
        //SDKTool.Singleton.PushEvent(EnumSDKEventType.eSDKEventType_Install_Success);
        //if (Application.internetReachability != NetworkReachability.NotReachable)
        //{
        //    //var checkNetwork = new CheckTimeout();
        //    //checkNetwork.AsynIsNetworkTimeout((result) =>
        //    //{
        //    //    TimerManager.AddTimer(0, 0, () =>
        //    //    {
        //    //        if (result)
        //    //        {
        //    //            //网络正常
        //    //            this.PreInstall();
        //    //        }
        //    //        else
        //    //        {
        //    //            Debug.LogError("网络不行");
        //    //        }
        //    //    });
        //    //});
           

        //}
    }
    private void PreInstall()
    {
        //DefaultUI.Instance.SetTipContent("数据加载中...");
        //if (UnityMonoDriver.s_instance.ReleaseMode)
        //{
        //    ResourceReadInfo.singleton.Init("ResourcesIndexInfo.txt", this.ResourceIndexInfoReadFinished);
        //}
        //else
        //{
            //DefaultUI.Instance.Release();
            //DefaultUI.Instance.ShowProgressBar(false);
            UnityMonoDriver.s_instance.LaterInit();
        //}
    }
    private void ResourceIndexInfoReadFinished()
    {
        //if (ResourceReadInfo.singleton.HasInited())
        //{
        //    Debug.Log("INit");
        //}
        //SteamAssetsManager.singleton.SetAllFinishedCallback(this.steamExportFinished);
        //SteamAssetsManager.singleton.FirstExport();
    }
    private void steamExportFinished()
    {
        //导出ab文件后，接下来就是版本检测相关的
        //DefaultUI.Instance.SetTipContent("版本检测中...");
        SDKTool.Singleton.PushEvent(EnumSDKEventType.eSDKEventType_Install_Success);
    }
    public virtual void Login()
    {
        //if (this.EPlatformType == EPlatformType.Platform_Tencent)
        //{
        //    shareSdk.Authorize(PlatformType.QQ);
        //    shareSdk.authHandler = AuthResultHandler;
        //}
    }
    //public void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable data)
    //{
    //    if (state == ResponseState.Success)
    //    {
    //        Hashtable userData = shareSdk.GetAuthInfo(PlatformType.QQ);
    //        //成功之后发送给游戏服务器，登录
    //        string userId = userData["userID"].ToString();
    //        string token = userData["token"].ToString();
    //        Debug.Log("UserId:" + userId);
    //        Debug.Log("Token:" + token);
    //        Singleton<Login>.singleton.StartPlatformLogin(userId, token);
    //    }
    //    else if (state == ResponseState.Fail)
    //    {

    //    }
    //    else if (state == ResponseState.Cancel)
    //    {

    //    }
    //}
    /*
     * {"refresh_token":"", 
     * "openID":"", 
     * "expiresIn":7776000,
     *  "userGender":"m", 
     *  "tokenSecret":"", 
     *  "userID":"746A3C255337DB38000E3AACD1F66C83", 
     *  "unionID":"", 
     *  "expiresTime":1520866376086,
     *   "userName":"\u8349\u5e3d", 
     *   "token":"472FA9B739EA18A7D911ED767074D4D5", 
     *   "userIcon":"http://q.qlogo.cn/qqapp/100371282/746A3C255337DB38000E3AACD1F66C83/100"}
     * 
     * */
}
