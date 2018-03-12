using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Threading;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CClientSocket
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class CClientSocket
    {
        private Socket m_oSocket;
        private SocketState m_nState;
        private byte[] m_oSendBuff;
        private byte[] m_oRecvBuff;
        private volatile int m_nStartPos;
        private volatile int m_nEndPos;
        private int m_nCurrRecvLen;
        private bool m_bSending;
        private CNetwork m_oNetwork;
        private IPacketBreaker m_oBreaker;
        private object m_oStateLock = new object();
        private object m_oSendingLock = new object();
        public CClientSocket()
        {
            this.m_oSocket = null;
            this.m_nState = SocketState.State_Closed;
            this.m_oSendBuff = null;
            this.m_oRecvBuff = null;
            this.m_nStartPos = 0;
            this.m_nEndPos = 0;
            this.m_nCurrRecvLen = 0;
            this.m_bSending = false;
            this.m_oSendBuff = null;
            this.m_oBreaker = null;
        }
        public bool Init(uint dwSendBuffSize, uint dwRecvBuffSize, CNetwork oNetwork, IPacketBreaker oBreaker)
        {
            this.m_nState = SocketState.State_Closed;
            this.m_oSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.m_oSendBuff = new byte[dwSendBuffSize];
            this.m_oRecvBuff = new byte[dwRecvBuffSize];
            this.m_nStartPos = 0;
            this.m_nEndPos = 0;
            this.m_nCurrRecvLen = 0;
            this.m_bSending = false;
            this.m_oNetwork = oNetwork;
            this.m_oSocket.SendBufferSize = (int)dwSendBuffSize;
            this.m_oSocket.ReceiveBufferSize = (int)dwRecvBuffSize;
            this.m_oBreaker = oBreaker;
            return true;
        }
        public void UnInit()
        {
            this.Close();
        }
        public bool Connect(string host, int port)
        {
            bool result;
            if (this.GetState() != SocketState.State_Closed)
            {
                result = false;
            }
            else
            {
                this.m_oNetwork.GetLogger().LogInfo("Begin Connect to host " + host + ":port " + port.ToString());
                try
                {
                    this.SetState(SocketState.State_Connecting);
                    this.m_oSocket.BeginConnect(host, port, new AsyncCallback(this.ConnectCallback), this.m_oSocket);
                }
                catch (Exception ex)
                {
                    this.SetState(SocketState.State_Closed);
                    this.m_oNetwork.GetLogger().LogError("Exception occured on BeginConnect: " + ex.Message);
                    result = false;
                    return result;
                }
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 异步连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            this.m_oNetwork.GetLogger().LogInfo("End Connect");
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
            }
            catch (Exception ex)
            {
                this.m_oNetwork.GetLogger().LogError("Exception occured on ConnectCallback: " + ex.Message);
            }
            if (socket.Connected)
            {
                this.GetNetwork().PushConnectEvent(true);//将连接成功的消息事件压入队列
                this.SetState(SocketState.State_Connected);//多线程设置状态为已经连接状态
                this.GetNetwork().m_dateTimeLastSend = DateTime.Now;
                this.BeginReceiveData();//开始异步接受数据
            }
            else
            {
                this.GetNetwork().PushConnectEvent(false);
                this.SetState(SocketState.State_Closed);
            }
        }
        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void Close()
        {
            if (this.m_oSocket != null)
            {
                if (this.GetState() != SocketState.State_Closed)
                {
                    this.SetState(SocketState.State_Closed);
                    this.SetSending(false);
                    try
                    {
                        if (this.m_oSocket.Connected)
                        {
                            this.m_oSocket.Shutdown(SocketShutdown.Both);
                        }
                        this.m_oSocket.Close();
                    }
                    catch (Exception ex)
                    {
                        this.m_oNetwork.GetLogger().LogError("Exception occured on Close: " + ex.Message);
                    }
                    this.m_oSocket = null;
                    this.m_oSendBuff = null;
                    this.m_oRecvBuff = null;
                    this.m_oNetwork.GetLogger().LogInfo("Close network connetion");
                }
            }
        }
        public bool Send(byte[] buffer)
        {
            return this.Send(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool Send(byte[] buffer, int start, int length)
        {
            bool result;
            if (this.GetState() != SocketState.State_Connected)
            {
                result = false;
            }
            else
            {
                if (length == 0)
                {
                    result = false;
                }
                else
                {
                    if (start + length > buffer.Length)
                    {
                        this.m_oNetwork.GetLogger().LogError("Send data buffer is invalid");
                        result = false;
                    }
                    else
                    {
                        int num = this.m_oSendBuff.Length;
                        int num2 = this.m_nEndPos + num - this.m_nStartPos;
                        int num3 = (num2 >= num) ? (num2 - num) : num2;
                        if (length + 1 + num3 > num)//溢出
                        {
                            this.Close();
                            this.GetNetwork().PushClosedEvent(NetErrCode.Net_SendBuff_Overflow);
                            result = false;
                        }
                        else
                        {
                            if (this.m_nEndPos + length >= num)//数据超过buffer，但是没有溢出，因为前面还有没用的数据可以覆盖
                            {
                                int num4 = num - this.m_nEndPos;
                                int num5 = length - num4;
                                Array.Copy(buffer, start, this.m_oSendBuff, this.m_nEndPos, num4);
                                Array.Copy(buffer, start + num4, this.m_oSendBuff, 0, num5);
                                this.m_nEndPos = num5;
                            }
                            else
                            {
                                Array.Copy(buffer, start, this.m_oSendBuff, this.m_nEndPos, length);
                                this.m_nEndPos += length;
                            }
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        public Socket GetSocket()
        {
            return this.m_oSocket;
        }
        public SocketState GetState()
        {
            Monitor.Enter(this.m_oStateLock);
            SocketState nState = this.m_nState;
            Monitor.Exit(this.m_oStateLock);
            return nState;
        }
        public void SetState(SocketState nState)
        {
            Monitor.Enter(this.m_oStateLock);
            this.m_nState = nState;
            Monitor.Exit(this.m_oStateLock);
        }
        private CNetwork GetNetwork()
        {
            return this.m_oNetwork;
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="nLen">数据长度</param>
        public void OnReceived(int nLen)
        {
            if (this.GetState() == SocketState.State_Connected)
            {
                if (0 == nLen)
                {
                    this.Close();
                    this.GetNetwork().PushClosedEvent(NetErrCode.Net_NoError);
                }
                else
                {
                    this.m_nCurrRecvLen += nLen;
                    int num = 0;
                    while (this.m_nCurrRecvLen > 0)
                    {
                        int num2 = this.m_oBreaker.BreakPacket(this.m_oRecvBuff, num, this.m_nCurrRecvLen);//黏包处理，返回一个完整数据的长度
                        if (num2 == 0)
                        {
                            break;
                        }
                        this.GetNetwork().WriteData(this.m_oRecvBuff, num, num2);
                        this.GetNetwork().PushReceiveEvent(num2);//将接受事件压入队列中
                        num += num2;
                        this.m_nCurrRecvLen -= num2;//因为已经写入piecebuffer中
                    }
                    if (this.m_nCurrRecvLen == this.m_oRecvBuff.Length)//收到数据溢出缓冲buffer
                    {
                        this.Close();
                        this.GetNetwork().PushClosedEvent(NetErrCode.Net_RecvBuff_Overflow);
                    }
                    else
                    {
                        if (num > 0 && this.m_nCurrRecvLen > 0)//如果收到数据不是个完整的数据，那么会将数据暂存到recvBuffer中，以下次读取
                        {
                            Array.Copy(this.m_oRecvBuff, num, this.m_oRecvBuff, 0, this.m_nCurrRecvLen);
                        }
                        this.BeginReceiveData();
                    }
                }
            }
        }
        /// <summary>
        /// 开始异步接受消息
        /// </summary>
        private void BeginReceiveData()
        {
            if (this.GetState() == SocketState.State_Connected)
            {
                int num = this.m_oRecvBuff.Length - this.m_nCurrRecvLen;
                if (num == 0)
                {
                    this.Close();
                    this.GetNetwork().PushClosedEvent(NetErrCode.Net_RecvBuff_Overflow);
                }
                else
                {
                    try
                    {
                        this.m_oSocket.BeginReceive(this.m_oRecvBuff, this.m_nCurrRecvLen, num, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), this.m_oSocket);
                    }
                    catch (Exception ex)
                    {
                        this.m_oNetwork.GetLogger().LogError("Exception occured on BeginReceive: " + ex.Message);
                        this.Close();
                        this.GetNetwork().PushClosedEvent(NetErrCode.Net_SysError);
                    }
                }
            }
        }
        /// <summary>
        /// 接受消息异步回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            SocketError socketError = SocketError.Success;
            int nLen = 0;
            try
            {
                nLen = socket.EndReceive(ar, out socketError);
            }
            catch (Exception ex)
            {
                this.m_oNetwork.GetLogger().LogError("Exception occured on EndReceive: " + ex.Message);
                this.Close();
                this.GetNetwork().PushClosedEvent(NetErrCode.Net_SysError);
                return;
            }
            this.OnReceived(nLen);
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        private void BeginSendData()
        {
            if (this.GetState() == SocketState.State_Connected)
            {
                int num = this.m_oSendBuff.Length;//发送数据缓存的长度
                int num2 = this.m_nEndPos + num - this.m_nStartPos;
                int num3 = (num2 >= num) ? (num2 - num) : num2;//实际需要发送的数据长度，主要是判断是否还有数据要发，根据结束指针-开始指针
                //如果结束比开始小，说明数据溢出了缓存，所以就为num2
                if (num3 == 0)
                {
                    this.SetSending(false);
                }
                else
                {
                    this.SetSending(true);
                    if (this.m_nStartPos + num3 >= num)
                    {
                        num3 = num - this.m_nStartPos;//如果数据超出了缓存，截取数据超出部分
                    }
                    try
                    {
                        this.m_oSocket.BeginSend(this.m_oSendBuff, this.m_nStartPos, num3, SocketFlags.None, new AsyncCallback(this.SendCallBack), this.m_oSocket);
                    }
                    catch (Exception ex)
                    {
                        this.m_oNetwork.GetLogger().LogError("Exception occured on BeginSend: " + ex.Message);
                        this.Close();
                        this.GetNetwork().PushClosedEvent(NetErrCode.Net_SysError);
                    }
                }
            }
        }
        /// <summary>
        /// 发送数据回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            SocketError socketError = SocketError.Success;
            int num = 0;
            try
            {
                num = socket.EndSend(ar, out socketError);
                //Debug.Log(num);
            }
            catch (Exception ex)
            {
                this.m_oNetwork.GetLogger().LogError("Exception occured on EndSend: " + ex.Message);
                this.Close();
                this.GetNetwork().PushClosedEvent(NetErrCode.Net_SysError);
                return;
            }
            if (socketError != SocketError.Success)
            {
                this.m_oNetwork.GetLogger().LogError("EndSend Error: " + socketError.ToString());
                this.Close();
                this.GetNetwork().PushClosedEvent(NetErrCode.Net_SysError);
            }
            else
            {
                this.m_nStartPos = (this.m_nStartPos + num) % this.m_oSendBuff.Length;//下次开始位置指针，也就是start+num（上次发送的长度）
                this.BeginSendData();
            }
        }
        /// <summary>
        /// 检测是否处于正在发送状态，不是就发送
        /// </summary>
        public void CheckBeginSend()
        {
            if (!this.IsSending())
            {
                this.BeginSendData();
            }
        }
        private void SetSending(bool bVal)
        {
            Monitor.Enter(this.m_oSendingLock);
            this.m_bSending = bVal;
            Monitor.Exit(this.m_oSendingLock);
        }
        /// <summary>
        /// 异步获取是否正在发送数据
        /// </summary>
        /// <returns></returns>
        private bool IsSending()
        {
            Monitor.Enter(this.m_oSendingLock);
            bool bSending = this.m_bSending;
            Monitor.Exit(this.m_oSendingLock);
            return bSending;
        }
    }
}