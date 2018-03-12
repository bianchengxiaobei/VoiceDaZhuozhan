using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CNetwork
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class CNetwork
    {
        private CClientSocket m_oSocket;
        private Queue m_oDataQueue;
        private INetProcess m_oProcess;
        private IPacketBreaker m_oBreaker;
        private uint m_dwSendBuffSize;
        private uint m_dwRecvBuffSize;
        private PieceBufferIO m_PieceBufferIO;
        private object m_objLockTimeLastReceive = new object();
        public DateTime m_dateTimeLastReceive;
        public DateTime m_dateTimeLastSend;
        private int m_nCheckLastReceiveCount;
        private CNetLogger m_oLogger;
        public CNetwork()
        {
            this.m_oSocket = null;
            this.m_oProcess = null;
            this.m_oLogger = null;
            this.m_dwSendBuffSize = 0u;
            this.m_dwRecvBuffSize = 0u;
        }
        public CNetLogger GetLogger()
        {
            return this.m_oLogger;
        }
        /// <summary>
        /// 初始化Network
        /// </summary>
        /// <param name="oProc"></param>
        /// <param name="oBreaker"></param>
        /// <param name="dwSendBuffSize">发送缓存数组</param>
        /// <param name="dwRecvBuffSize">接收缓存数组</param>
        /// <param name="strLogDir">network.log的目录</param>
        /// <param name="bLogEnabled">是否启动log</param>
        /// <returns></returns>
        public bool Init(INetProcess oProc, IPacketBreaker oBreaker, uint dwSendBuffSize, uint dwRecvBuffSize, string strLogDir, bool bLogEnabled)
        {
            this.m_oProcess = oProc;
            this.m_oBreaker = oBreaker;
            this.m_dwSendBuffSize = dwSendBuffSize;
            this.m_dwRecvBuffSize = dwRecvBuffSize;
            this.m_PieceBufferIO = new PieceBufferIO();
            this.m_oDataQueue = new Queue();
            this.m_oLogger = new CNetLogger(strLogDir, bLogEnabled);
            this.m_oLogger.LogInfo("-----------------------------");
            this.m_oLogger.LogInfo("Init network succ");
            return true;
        }
        public void UnInit()
        {
            this.m_oLogger.LogInfo("UnInit network");
            this.Close();
            this.m_oLogger.Close();
        }
        /// <summary>
        /// 连接服务器，初始化CCClientSocket，开始异步连接
        /// </summary>
        /// <param name="host">端口</param>
        /// <param name="port">ip地址</param>
        /// <returns>是否连接成功</returns>
        public bool Connect(string host, int port)
        {
            this.Close();
            this.m_oSocket = new CClientSocket();
            bool result;
            if (!this.m_oSocket.Init(this.m_dwSendBuffSize, this.m_dwRecvBuffSize, this, this.m_oBreaker))
            {
                this.m_oSocket = null;
                result = false;
            }
            else
            {
                result = this.m_oSocket.Connect(host, port);
            }
            return result;
        }
        /// <summary>
        /// 关闭连接，释放Socket
        /// </summary>
        public void Close()
        {
            if (this.m_oSocket != null)
            {
                this.m_oSocket.Close();
                this.m_oSocket = null;
            }
        }
        /// <summary>
        /// 多线程下获取Socket状态，用Monitor
        /// </summary>
        /// <returns></returns>
        public SocketState GetState()
        {
            SocketState result;
            if (this.m_oSocket != null)
            {
                result = this.m_oSocket.GetState();
            }
            else
            {
                result = SocketState.State_Closed;
            }
            return result;
        }
        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="buffer">byte数据</param>
        /// <returns>是否发送成功</returns>
        public bool Send(byte[] buffer)
        {
            return this.m_oSocket.Send(buffer);
        }
        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="buffer">byte数据</param>
        /// <param name="start">开始位置index</param>
        /// <param name="length">长度数据</param>
        /// <returns>是否发送成功</returns>
        public bool Send(byte[] buffer, int start, int length)
        {
            return this.m_oSocket != null && this.m_oSocket.Send(buffer, start, length);
        }
        /// <summary>
        /// 处理收发消息
        /// </summary>
        /// <returns>处理的消息数</returns>
        public int ProcessMsg()
        {
            int result;
            if (this.m_oProcess == null || this.m_oSocket == null)
            {
                result = 0;
            }
            else
            {
                if (this.GetState() == SocketState.State_Connected)
                {
                    this.m_oSocket.CheckBeginSend();//也就是fixedUpdate每帧都在执行，在NetworkManager下
                }
                int num = 0;
                object obj = this.DeQueue();//消息事件出队列
                while (obj != null)
                {
                    NetEvent netEvent = obj as NetEvent;
                    if (netEvent == null)
                    {
                        Trace.Assert(false);
                    }
                    else
                    {
                        switch (netEvent.m_nEvtType)
                        {
                            case NetEvtType.Event_Connect:
                                this.m_oProcess.OnConnect(netEvent.m_bSuccess);
                                break;
                            case NetEvtType.Event_Closed:
                                this.m_oProcess.OnClosed(netEvent.m_nErrCode);
                                break;
                            case NetEvtType.Event_Receive:
                                this.m_oProcess.OnReceive(netEvent.m_dataLen);
                                break;
                            default:
                                Trace.Assert(false);
                                break;
                        }
                        CInstacePoolKeepNumber<NetEvent>.Release(ref netEvent);
                        num++;
                        obj = this.DeQueue();
                    }
                }
                result = num;
            }
            return result;
        }
        public void CheckHeartBeat(bool bIsMobile)
        {
            if (this.GetState() == SocketState.State_Connected && null != this.m_oProcess)
            {
                try
                {
                    if (bIsMobile)
                    {
                        Monitor.Enter(this.m_objLockTimeLastReceive);
                        TimeSpan timeSpan = DateTime.Now - this.m_dateTimeLastReceive;
                        Monitor.Exit(this.m_objLockTimeLastReceive);
                        if (timeSpan.TotalSeconds > 10.0)
                        {
                            if (++this.m_nCheckLastReceiveCount > 100)
                            {
                                this.m_nCheckLastReceiveCount = 0;
                                if (null != this.m_oSocket)
                                {
                                    this.m_oSocket.SetState(SocketState.State_Closed);
                                }
                                this.PushClosedEvent(NetErrCode.Net_OutTime);
                            }
                        }
                        else
                        {
                            this.m_nCheckLastReceiveCount = 0;
                        }
                    }
                    if ((DateTime.Now - this.m_dateTimeLastSend).TotalSeconds > 5.0)
                    {
                        //Debug.Log("发送心跳");
                        if (null != this.m_oProcess)
                        {
                            this.m_oProcess.SendKeepAlivePacket();
                            //if (Login.singleton.IsLogined)
                            //{
                            //    NetworkManager.singleton.SendMsg(new ReqAskPingMessage());
                            //}                       
                        }
                        this.m_dateTimeLastSend = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    this.m_oLogger.LogError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 多线程消息事件入队列
        /// </summary>
        /// <param name="evt"></param>
        public void EnQueue(object evt)
        {
            if (evt != null)
            {
                Monitor.Enter(this.m_oDataQueue);
                this.m_oDataQueue.Enqueue(evt);
                Monitor.Exit(this.m_oDataQueue);
            }
        }
        /// <summary>
        /// 多线程消息事件出队列
        /// </summary>
        /// <returns></returns>
        private object DeQueue()
        {
            object result = null;
            Monitor.Enter(this.m_oDataQueue);
            if (this.m_oDataQueue.Count > 0)
            {
                result = this.m_oDataQueue.Dequeue();
            }
            Monitor.Exit(this.m_oDataQueue);
            return result;
        }
        private void ClearQueue()
        {
            Monitor.Enter(this.m_oDataQueue);
            this.m_oDataQueue.Clear();
            Monitor.Exit(this.m_oDataQueue);
        }
        /// <summary>
        /// 连接事件压入队列
        /// </summary>
        /// <param name="bSuccess">是否连接成功</param>
        public void PushConnectEvent(bool bSuccess)
        {
            NetEvent netEvent = CInstacePoolKeepNumber<NetEvent>.Alloc();
            netEvent.m_nEvtType = NetEvtType.Event_Connect;
            netEvent.m_bSuccess = bSuccess;
            this.EnQueue(netEvent);
        }
        /// <summary>
        /// 关闭事件压入队列
        /// </summary>
        /// <param name="nErrCode">关闭错误码</param>
        public void PushClosedEvent(NetErrCode nErrCode)
        {
            NetEvent netEvent = CInstacePoolKeepNumber<NetEvent>.Alloc();
            netEvent.m_nEvtType = NetEvtType.Event_Closed;
            netEvent.m_nErrCode = nErrCode;
            this.EnQueue(netEvent);
        }
        /// <summary>
        /// 接收事件压入队列
        /// </summary>
        /// <param name="oData">byte数据</param>
        public void PushReceiveEvent(byte[] oData)
        {
            this.EnQueue(new NetEvent
            {
                m_nEvtType = NetEvtType.Event_Receive,
                m_oBuffer = oData
            });
        }
        /// <summary>
        /// 接收事件压入队列
        /// </summary>
        /// <param name="dataLen">数据长度</param>
        public void PushReceiveEvent(int dataLen)
        {
            NetEvent netEvent = CInstacePoolKeepNumber<NetEvent>.Alloc();
            netEvent.m_nEvtType = NetEvtType.Event_Receive;
            netEvent.m_dataLen = dataLen;
            this.EnQueue(netEvent);
        }
        /// <summary>
        /// 将收到的消息写到PieceBuffer中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        public void WriteData(byte[] data, int offset, int len)
        {
            Monitor.Enter(this.m_PieceBufferIO);
            this.m_PieceBufferIO.Write(data, offset, len);
            Monitor.Exit(this.m_PieceBufferIO);
        }
        /// <summary>
        /// 从PieceBuffer中读取消息，并且存到LinkedList中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        public void ReadData(byte[] data, int offset, int len)
        {
            Monitor.Enter(this.m_PieceBufferIO);
            this.m_PieceBufferIO.Read(data, offset, len);
            Monitor.Exit(this.m_PieceBufferIO);
        }
    }
}