using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：NetEvent
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    [AInstanceNumber(32)]
    public class NetEvent : IInstancePoolObject
    {
        /// <summary>
        /// Socket事件类型
        /// </summary>
        public NetEvtType m_nEvtType;
        public byte[] m_oBuffer;
        public bool m_bSuccess;
        /// <summary>
        /// Socket错误码
        /// </summary>
        public NetErrCode m_nErrCode;
        public int m_dataLen;
        public void OnAlloc()
        {
 
        }
        public void OnRelease()
        {
            this.m_oBuffer = null;
        }
    }
}