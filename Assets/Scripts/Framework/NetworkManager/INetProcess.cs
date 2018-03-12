using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：INetProcess
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public interface INetProcess
    {
        void OnConnect(bool bSuccess);
        void OnClosed(NetErrCode nErrCode);
        void OnReceive(byte[] data);
        void OnReceive(int dataLen);
        bool SendKeepAlivePacket();
    }
}
