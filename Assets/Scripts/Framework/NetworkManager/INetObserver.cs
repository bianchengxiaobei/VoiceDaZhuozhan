using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：INetObserver
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public interface INetObserver
    {
        void OnConnect(bool bSuccess);
        void OnClosed(NetErrCode nErrCode);
        void OnReceive(int dwType, int nLen);
        void OnSend(int dwType, int nLen);
    }
}

