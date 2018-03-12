using UnityEngine;
using System;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：AInstanceNumber
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework

{
    [AttributeUsage(AttributeTargets.Class)]
    public class AInstanceNumber : Attribute
    {
        public int Num { get; private set; }
        public AInstanceNumber(int num)
        {
            this.Num = num;
        }
    }
}
