using UnityEngine;
using System;
using System.Diagnostics;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：PieceBuffer
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class PieceBuffer
    {
        private byte[] m_buffer;
        private int m_nWriteOffset = 0;//记录当前buffer中实际写入数据的位置
        private int m_nReadOffset = 0;//记录当前buffer中实际读取数据的位置
        /// <summary>
        /// 读取数据是否溢出
        /// </summary>
        public bool IsReadOver
        {
            get
            {
                return this.m_nReadOffset == this.m_buffer.Length;//判断当前已经读取的偏移量和buffer的长度比较，如果相等的话就说明溢出了
            }
        }
        /// <summary>
        /// 写入的数据是否溢出
        /// </summary>
        public bool IsWriteOver
        {
            get
            {
                return this.m_nWriteOffset == this.m_buffer.Length;
            }
        }
        public PieceBuffer(int size)
        {
            this.m_buffer = new byte[size];
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns>读取的长度</returns>
        public int Read(byte[] buffer, int offset, int len)
        {
            Trace.Assert(len > 0 && offset + len <= buffer.Length);
            int num = this.m_buffer.Length - this.m_nReadOffset;
            int num2 = (num > len) ? len : num;
            Array.Copy(this.m_buffer, this.m_nReadOffset, buffer, offset, num2);
            this.m_nReadOffset += num2;
            return num2;
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns>写入的长度</returns>
        public int Write(byte[] buffer, int offset, int len)
        {
            Trace.Assert(len > 0 && offset + len <= buffer.Length);
            int num = this.m_buffer.Length - this.m_nWriteOffset;//buffer剩余的长度
            int num2 = (num > len) ? len : num;//实际写入的长度，如果剩余长度小于要写的长度，那么就只能写剩余的长度
            Array.Copy(buffer, offset, this.m_buffer, this.m_nWriteOffset, num2);
            this.m_nWriteOffset += num2;
            return num2;
        }
        public void Reset()
        {
            this.m_nReadOffset = (this.m_nWriteOffset = 0);
        }
    }
}
