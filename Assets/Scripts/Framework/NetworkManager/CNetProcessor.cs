using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;
using Debug = UnityEngine.Debug;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CNetProcessor
// 创建者：chen
// 修改者列表：
// 创建日期：2016.1.28
// 模块描述：消息处理器
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class CNetProcessor : INetProcess
    {
        private CNetwork m_oNetwork;
        private INetObserver m_oObserver;
        private CByteStream m_oSendStream;
        private CByteStream m_oRecvStream;
        private byte[] m_procDataBuffer;
        private static object obj = new object();
        public static Int32 order = 0;
        public CNetwork Network
        {
            get
            {
                return this.m_oNetwork;
            }
            set
            {
                this.m_oNetwork = value;
            }
        }
        public INetObserver Observer
        {
            set
            {
                this.m_oObserver = value;
            }
        }
        public CNetProcessor()
        {
            this.m_oNetwork = null;
            this.m_oObserver = null;
            this.m_oSendStream = new CByteStream();
            this.m_oRecvStream = new CByteStream();
            this.m_procDataBuffer = new byte[10240];
        }
        public void OnConnect(bool bSuccess)
        {
            if (bSuccess)
            {
                Console.WriteLine("Connect sucess");
            }
            else
            {
                Console.WriteLine("Connect failed");
            }
            if (this.m_oObserver != null)
            {
                this.m_oObserver.OnConnect(bSuccess);
            }
        }
        public void OnClosed(NetErrCode nErrCode)
        {
            order = 0;
            if (this.m_oObserver != null)
            {
                this.m_oObserver.OnClosed(nErrCode);
            }
        }
        public void OnReceive(byte[] data)
        {
        }
        public void OnReceive(int dataLen)
        {
            if (dataLen < 8)//前4个字节是总长度，后四个字节是协议类型
            {
                Trace.Assert(false);
            }
            else
            {
                byte[] array = this.m_procDataBuffer;
                if (dataLen > this.m_procDataBuffer.Length)
                {
                    array = new byte[dataLen];//如果长度不够，扩充字节数字
                }
                this.m_oNetwork.ReadData(array, 0, dataLen);//读从PieceBuffer中读取收到的数据
                int dwType = BitConverter.ToInt32(array, 4);//从第4个字节index开始，读取一个int值
                //Debug.Log("消息Id:" + dwType);
                CProtocol protocol = CProtocol.GetProtocol(dwType);
                if (protocol == null)
                {
                    Trace.Assert(false);
                    Debug.Log("没有找到消息");
                }
                else
                {
                    this.m_oRecvStream.Clear();
                    this.m_oRecvStream.SetBuffer(array, 8, dataLen - 8);
                    //Debug.Log(dataLen);
                    protocol.DeSerialize(this.m_oRecvStream);//反序列化
                    protocol.Process();
                    if (this.m_oObserver != null)
                    {
                        this.m_oObserver.OnReceive(dwType, dataLen);
                    }
                }
            }
        }
        /// <summary>
        /// 发送协议消息
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public bool Send(CProtocol protocol)
        {
            Int32 num = 0;
            this.m_oSendStream.Clear();
            this.m_oSendStream.Write(num);//写入长度
            int size = this.m_oSendStream.GetSize();
            Monitor.Enter(obj);
            this.m_oSendStream.Write(order++);//写入发送序列
            Monitor.Exit(obj);
            this.m_oSendStream.Write(protocol.Type);//类型id
            protocol.Serialize(this.m_oSendStream);//真实协议数据
            int size2 = this.m_oSendStream.GetSize();
            num = size2 - size;
            this.m_oSendStream.Copy(BitConverter.GetBytes(num), 0);//copy头部总长度到发送流中
            bool flag = this.m_oNetwork.Send(this.m_oSendStream.GetBuffer(), 0, this.m_oSendStream.GetSize());
            if (flag && this.m_oObserver != null)
            {
                this.m_oObserver.OnSend(protocol.Type, this.m_oSendStream.GetSize());
            }
            return flag;
        }
        public bool SendKeepAlivePacket()
        {
            int len = 8;
            int id = 0;
            this.m_oSendStream.Clear();
            this.m_oSendStream.Write(len);
            Monitor.Enter(obj);
            this.m_oSendStream.Write(order++);//写入发送序列
            Monitor.Exit(obj);
            this.m_oSendStream.Write(id);
            bool flag = this.m_oNetwork.Send(this.m_oSendStream.GetBuffer(), 0, this.m_oSendStream.GetSize());
            if (flag && this.m_oObserver != null)
            {
                this.m_oObserver.OnSend(0, this.m_oSendStream.GetSize());
            }
            return flag;
        }
    }
}
