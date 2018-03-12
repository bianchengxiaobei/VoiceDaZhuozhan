using UnityEngine;
using System.Collections;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：IXLog
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public interface IXLog
    {
        void Debug(object message);
        void Info(object message);
        void Error(object message);
        void Fatal(object message);
    }
    /// <summary>
    /// 调试级别，Debug，Error，Info等等
    /// </summary>
    public enum EnumLogLevel
    {
        eLogLevel_Debug,
        eLogLevel_Info,
        eLogLevel_Error,
        eLogLevel_Fatal,
        eLogLevel_Max
    }
}
