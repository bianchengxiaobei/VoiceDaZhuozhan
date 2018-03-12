using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CInstacePoolKeepNumber
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public static class CInstacePoolKeepNumber<T> where T :class,IInstancePoolObject,new()
    {
        private static T[] m_activeInstances;
        private static Stack<T> m_inactiveInstances;
        static CInstacePoolKeepNumber()
        {
            Type typeFromHandle = typeof(T);
            object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(AInstanceNumber), true);
            if (customAttributes != null && customAttributes.Length != 0)
            {
                AInstanceNumber aInstanceNumber = customAttributes[0] as AInstanceNumber;
                CInstacePoolKeepNumber<T>.m_activeInstances = new T[aInstanceNumber.Num];
                CInstacePoolKeepNumber<T>.m_inactiveInstances = new Stack<T>(aInstanceNumber.Num);
            }
        }
        /// <summary>
        /// 分配一次内存
        /// </summary>
        /// <returns></returns>
        public static T Alloc()
        {
            T t = default(T);
            Stack<T> inactiveInstances;
            Monitor.Enter(inactiveInstances = CInstacePoolKeepNumber<T>.m_inactiveInstances);
            try
            {
                t = (CInstacePoolKeepNumber<T>.m_inactiveInstances.Count > 0 ? CInstacePoolKeepNumber<T>.m_inactiveInstances.Pop() : Activator.CreateInstance<T>());
            }
            finally 
            {
                Monitor.Exit(inactiveInstances);
            }
            T[] activeInstances;
            Monitor.Enter(activeInstances = CInstacePoolKeepNumber<T>.m_activeInstances);
            try
            {
                for (int i = 0; i < CInstacePoolKeepNumber<T>.m_activeInstances.Length; i++)
                {
                    if (null == CInstacePoolKeepNumber<T>.m_activeInstances[i])
                    {
                        CInstacePoolKeepNumber<T>.m_activeInstances[i] = t;
                        break;
                    }
                }
            }
            finally
            {
                Monitor.Exit(activeInstances);
            }
            t.OnAlloc();
            return t;
        }
        /// <summary>
        /// 释放一次内存，存到Stack<inactiveInstance>中
        /// </summary>
        /// <param name="obj"></param>
        public static void Release(ref T obj)
        {
            if (null != obj)
            {
                obj.OnRelease();
                T[] activeInstances;
                Monitor.Enter(activeInstances = CInstacePoolKeepNumber<T>.m_activeInstances);
                try
                {
                    for (int i = 0; i < CInstacePoolKeepNumber<T>.m_activeInstances.Length; i++)
                    {
                        if (object.ReferenceEquals(CInstacePoolKeepNumber<T>.m_activeInstances[i], obj))
                        {
                            CInstacePoolKeepNumber<T>.m_activeInstances[i] = default(T);
                            Stack<T> inactiveInstances;
                            Monitor.Enter(inactiveInstances = CInstacePoolKeepNumber<T>.m_inactiveInstances);
                            try
                            {
                                CInstacePoolKeepNumber<T>.m_inactiveInstances.Push(obj);
                            }
                            finally 
                            {
                                Monitor.Exit(inactiveInstances);
                            }
                            break;
                        }
                    }
                }
                finally 
                {
                    Monitor.Exit(activeInstances);
                }
                obj = default(T);
            }
        }
    }
}
 