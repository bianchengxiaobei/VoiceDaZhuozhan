using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：IInstancePoolObject
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public interface IInstancePoolObject
    {
        /// <summary>
        /// 分配内存
        /// </summary>
        void OnAlloc();
        /// <summary>
        /// 释放内存
        /// </summary>
        void OnRelease();
    }
}