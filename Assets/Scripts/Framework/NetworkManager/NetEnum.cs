using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：NetEnum
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    /// <summary>
    /// Socket状态
    /// </summary>
    public enum SocketState
    {
        State_Closed,
        State_Connecting,
        State_Connected
    }
    /// <summary>
    /// Socket事件
    /// </summary>
    public enum NetEvtType
    {
        Event_Connect,
        Event_Closed,
        Event_Receive
    }
    /// <summary>
    /// Socket错误码
    /// </summary>
    public enum NetErrCode
    {
        Net_NoError,
        Net_SysError,
        Net_RecvBuff_Overflow,
        Net_SendBuff_Overflow,
        Net_Unknown_Exception,
        Net_OutTime
    }
}
