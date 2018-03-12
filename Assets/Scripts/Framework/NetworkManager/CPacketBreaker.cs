using UnityEngine;
using System;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CPacketBreaker
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class CPacketBreaker : IPacketBreaker
    {
        public int BreakPacket(byte[] data, int index, int len)
        {
            int result;
            if (len < 4)
            {
                result = 0;
            }
            else
            {
                int num = BitConverter.ToInt32(data, index);
                if (len < 4 + num)
                {
                    result = 0;
                }
                else
                {
                    result = num + 4;
                }
            }
            return result;
        }
    }
}

