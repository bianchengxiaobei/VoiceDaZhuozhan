using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MsgPack;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CByteStream
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework

{
    public class CByteStream
    {
        private byte[] m_oBuffer;
        
        private int m_nReadPos;
        private int m_nSize;
        private int m_nCapacity;
        public byte[] TempBuffer = new byte[1024];
        public CByteStream()
        {
            this.m_nSize = 0;
            this.m_nReadPos = 0;
            this.m_nCapacity = 256;
            this.m_oBuffer = new byte[this.m_nCapacity];
        }
        public CByteStream(byte[] data)
        {
            this.m_nReadPos = 0;
            this.m_nSize = 0;
            this.m_nCapacity = data.Length;
            this.m_oBuffer = data;
        }
        public void SetBuffer(byte[] data)
        {
            this.m_nReadPos = 0;
            this.m_nSize = 0;
            this.m_nCapacity = data.Length;
            this.m_oBuffer = data;
        }
        public void SetBuffer(byte[] data, int offset, int len)
        {
            Trace.Assert(offset + len <= data.Length);
            this.m_nReadPos = offset;
            this.m_nSize = data.Length;
            this.m_nCapacity = data.Length;
            this.m_oBuffer = data;
        }
        public void Assign(byte[] data)
        {
            this.Assign(data, 0, data.Length);
        }
        public void Assign(byte[] data, int offset, int len)
        {
            Trace.Assert(offset + len <= data.Length);
            this.Clear();
            if (len > 0)
            {
                this.SpaceCheck(len);
                Array.Copy(data, offset, this.m_oBuffer, this.m_nSize, len);
                this.m_nSize = len;
            }
        }
        public void Copy(byte[] data, int destIndex)
        {
            if (destIndex + data.Length <= this.m_nSize)
            {
                Array.Copy(data, 0, this.m_oBuffer, destIndex, data.Length);
            }
        }
        public void Clear()
        {
            this.m_nReadPos = 0;
            this.m_nSize = 0;
        }
        public byte[] GetBuffer()
        {
            return this.m_oBuffer;
        }
        public int GetSize()
        {
            return this.m_nSize;
        }
        public CByteStream Write(sbyte val)
        {
            this.SpaceCheck(1);
            byte[] bytes = BitConverter.GetBytes((short)val);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 1);
            this.m_nSize++;
            return this;
        }
        public CByteStream Write(byte val)
        {
            this.SpaceCheck(1);
            byte[] bytes = BitConverter.GetBytes((short)val);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 1);
            this.m_nSize++;
            return this;
        }
        public CByteStream Write(ByteArrayPacker packer, byte val)
        {
            packer.Pack(val);
            this.m_nSize += packer.BytesUsed;
            return this;
        }
        public CByteStream Write(byte[] val)
        {
            this.SpaceCheck(val.Length);
            Array.Copy(val, 0, this.m_oBuffer, this.m_nSize, val.Length);
            this.m_nSize += val.Length;
            return this;
        }
        public CByteStream Write(byte[] val,int len, int offset = 0)
        {
            this.SpaceCheck(len);
            Array.Copy(val, offset, this.m_oBuffer, this.m_nSize, len);
            this.m_nSize += len;
            return this;
        }
        public CByteStream Write(short val)
        {
            this.SpaceCheck(2);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 2);
            this.m_nSize += 2;
            return this;
        }
        public CByteStream Write(ushort val)
        {
            this.SpaceCheck(2);
            byte[] bytes = BitConverter.GetBytes(val);
            //Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 2);
            this.m_nSize += 2;
            return this;
        }
        public CByteStream Write(int val)
        {
            this.SpaceCheck(4);
            byte[] bytes = BitConverter.GetBytes(val);
           // Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 4);
            this.m_nSize += 4;
            return this;
        }
        public CByteStream Write(ByteArrayPacker packer, int val)
        {
            packer.Pack(val);
            this.m_nSize += packer.BytesUsed;
            return this;
        }
        public CByteStream Write(uint val)
        {
            this.SpaceCheck(4);
            byte[] bytes = BitConverter.GetBytes(val);
           // Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 4);
            this.m_nSize += 4;
            return this;
        }
        public CByteStream Write(long val)
        {
            this.SpaceCheck(8);
            byte[] bytes = BitConverter.GetBytes(val);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 8);
            this.m_nSize += 8;
            return this;
        }
        public CByteStream Write(ulong val)
        {
            this.SpaceCheck(8);
            byte[] bytes = BitConverter.GetBytes(val);
           // Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 8);
            this.m_nSize += 8;
            return this;
        }
        public CByteStream Write(float val)
        {
            this.SpaceCheck(4);
            byte[] bytes = BitConverter.GetBytes(val);
            //Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 4);
            this.m_nSize += 4;
            return this;
        }
        public CByteStream Write(double val)
        {
            this.SpaceCheck(8);
            byte[] bytes = BitConverter.GetBytes(val);
           // Array.Reverse(bytes);
            Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, 8);
            this.m_nSize += 8;
            return this;
        }
        public CByteStream Write(string str)
        {
            CByteStream result;
            if (str.Length == 0)
            {
                this.Write(str.Length);
                result = this;
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                this.Write(bytes.Length);
                this.SpaceCheck(bytes.Length);
                Array.Copy(bytes, 0, this.m_oBuffer, this.m_nSize, bytes.Length);
                this.m_nSize += bytes.Length;
                result = this;
            }
            return result;
        }
        public CByteStream Write(ByteArrayPacker packer, string val)
        {
            packer.PackString(val);
            this.m_nSize += packer.BytesUsed;
            return this;
        }
        public CByteStream Read(ref sbyte val)
        {
            if (this.m_nReadPos + 1 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = (sbyte)this.m_oBuffer[this.m_nReadPos];
            this.m_nReadPos++;
            return this;
        }
        public CByteStream Read(ref byte val)
        {
            if (this.m_nReadPos + 1 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = this.m_oBuffer[this.m_nReadPos];
            this.m_nReadPos++;
            return this;
        }
        public CByteStream Read(ref short val)
        {
            if (this.m_nReadPos + 2 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToInt16(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 2;
            return this;
        }
        public CByteStream Read(ref ushort val)
        {
            if (this.m_nReadPos + 2 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToUInt16(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 2;
            return this;
        }
        public CByteStream Read(ref int val)
        {
            if (this.m_nReadPos + 4 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToInt32(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 4;
            return this;
        }
        public CByteStream Read(ref uint val)
        {
            if (this.m_nReadPos + 4 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToUInt32(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 4;
            return this;
        }
        public CByteStream Read(ref long val)
        {
            if (this.m_nReadPos + 8 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToInt64(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 8;
            return this;
        }
        public CByteStream Read(ref ulong val)
        {
            if (this.m_nReadPos + 8 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToUInt64(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 8;
            return this;
        }
        public CByteStream Read(ref float val)
        {
            if (this.m_nReadPos + 4 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToSingle(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 4;
            return this;
        }
        public CByteStream Read(ref double val)
        {
            if (this.m_nReadPos + 8 > this.m_nSize)
            {
                throw new Exception("There is no more data to read.");
            }
            val = BitConverter.ToDouble(this.m_oBuffer, this.m_nReadPos);
            this.m_nReadPos += 8;
            return this;
        }
        public CByteStream Read(ref string str)
        {
            int num = 0;
            this.Read(ref num);
            CByteStream result;
            if (num == 0)
            {
                str = "";
                result = this;
            }
            else
            {
                if (this.m_nReadPos + num > this.m_nSize)
                {
                    throw new Exception("There is no more data to read.");
                }
                str = Encoding.UTF8.GetString(this.m_oBuffer, this.m_nReadPos, num);
                this.m_nReadPos += num;
                result = this;
            }
            return result;
        }
        private void SpaceCheck(int nDataLen)
        {
            if (this.m_nSize + nDataLen > this.m_nCapacity)
            {
                int num = (this.m_nSize + nDataLen) * 2;
                byte[] array = new byte[num];
                Array.Copy(this.m_oBuffer, 0, array, 0, this.m_nSize);
                this.m_oBuffer = array;
                this.m_nCapacity = num;
            }
        }
        public CByteStream Write(List<sbyte> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<sbyte> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                sbyte item = 0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<byte> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<byte> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                byte item = 0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<short> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<short> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                short item = 0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<ushort> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<ushort> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                ushort item = 0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<int> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<int> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                int item = 0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<uint> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<uint> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                uint item = 0u;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<long> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<long> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                long item = 0L;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<ulong> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<ulong> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                ulong item = 0uL;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<float> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<float> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                float item = 0f;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<double> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<double> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                double item = 0.0;
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(List<string> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
        public CByteStream Read(List<string> list)
        {
            list.Clear();
            int num = 0;
            this.Read(ref num);
            for (int i = 0; i < num; i++)
            {
                string item = "";
                this.Read(ref item);
                list.Add(item);
            }
            return this;
        }
        public CByteStream Write(IData data)
        {
            return data.Serialize(this);
        }
        public CByteStream Write(List<IData> list)
        {
            this.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                this.Write(list[i]);
            }
            return this;
        }
    }
}
