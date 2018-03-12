using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CProtocol
// 创建者：chen
// 修改者列表：
// 创建日期：2016.1.28
// 模块描述：协议抽象类
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public abstract class CProtocol
    {
        private int m_dwType;
        private static Dictionary<int, CProtocol> sm_oPtcMap;
        /// <summary>
        /// 协议类型
        /// </summary>
        public int Type
        {
            get
            {
                return this.m_dwType;
            }
            set
            {
                this.m_dwType = value;
            }
        }
        public CProtocol(int dwType)
        {
            this.m_dwType = dwType;
        }
        static CProtocol()
        {
            CProtocol.sm_oPtcMap = new Dictionary<int, CProtocol>();
        }
        /// <summary>
        /// 注册协议
        /// </summary>
        /// <param name="ptc"></param>
        /// <returns></returns>
        public static bool Register(CProtocol ptc)
        {
            bool result;
            if (CProtocol.sm_oPtcMap.ContainsKey(ptc.Type))
            {
                Trace.Assert(false);
                result = false;
            }
            else
            {
                CProtocol.sm_oPtcMap.Add(ptc.Type, ptc);
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 静态方法，从已经注册的协议中得到这个类型的协议对象
        /// </summary>
        /// <param name="dwType">协议类型，为uint</param>
        /// <returns>协议对象</returns>
        public static CProtocol GetProtocol(int dwType)
        {
            CProtocol result = null;
            CProtocol.sm_oPtcMap.TryGetValue(dwType, out result);
            return result;
        }
        public abstract void Serialize(CByteStream bs);
        public abstract void DeSerialize(CByteStream bs);
        public abstract void Process();
    }
}
