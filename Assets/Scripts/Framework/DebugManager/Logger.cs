using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：Logger
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：调试信息类
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class Logger : IXLog
    {
        private string m_strTag = string.Empty;
        private static FileStream s_ofs;
        private static StreamWriter s_owr;
        private static UdpClient s_udpClient;
        private static int s_nLastSaveTime;
        private static EnumLogLevel s_eLogLevel;
        public static bool IsInEditor
        {
            get;
            set;
        }
        public static EnumLogLevel LogLevel
        {
            get
            {
                return Logger.s_eLogLevel;
            }
            set
            {
                Logger.s_eLogLevel = value;
            }
        }
        public Logger(string strTag)
        {
            this.m_strTag = strTag;
        }
        public static bool Init(string strFilePath)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(strFilePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                Logger.s_ofs = new FileStream(strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Logger.s_owr = new StreamWriter(Logger.s_ofs, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.s_owr = null;
                UnityEngine.Debug.LogError(ex.ToString());
                if (Logger.s_udpClient != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(ex.ToString());
                    Logger.s_udpClient.Send(bytes, bytes.Length);
                }
                return false;
            }
            return true;
        }
        public static bool Init(string strRemoteIP, int nPort)
        {
            try
            {
                Logger.s_udpClient = new UdpClient();
                Logger.s_udpClient.Connect(strRemoteIP, nPort);
            }
            catch (Exception ex)
            {
                Logger.s_udpClient = null;
                UnityEngine.Debug.LogError(ex.ToString());
                return false;
            }
            return true;
        }
        public static void Close()
        {
            Logger.Save();
            if (Logger.s_owr != null)
            {
                Logger.s_owr.Close();
                Logger.s_owr = null;
            }
            if (Logger.s_ofs != null)
            {
                Logger.s_ofs.Close();
                Logger.s_ofs = null;
            }
        }
        private static void Save()
        {
            Logger.s_nLastSaveTime = Environment.TickCount;
            if (Logger.s_owr != null)
            {
                Logger.s_owr.Flush();
            }
            if (Logger.s_ofs != null)
            {
                Logger.s_ofs.Flush();
            }
        }
        private static void AddInfo(string strInfo)
        {
            try
            {
                if (Logger.s_udpClient != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", strInfo));
                    Logger.s_udpClient.Send(bytes, bytes.Length);
                }
            }
            catch (Exception)
            {
            }
            if (Logger.s_owr != null)
            {
                Logger.s_owr.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss,ffff} {1}", DateTime.Now, strInfo));
                if (Environment.TickCount - Logger.s_nLastSaveTime >= 5000)
                {
                    Logger.Save();
                }
            }
        }
        public void Debug(object message)
        {
            if (EnumLogLevel.eLogLevel_Debug < Logger.s_eLogLevel)
            {
                return;
            }
            string text = string.Format("[Debug] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
                return;
            }
            UnityEngine.Debug.Log(text);
        }
        public void Info(object message)
        {
            if (EnumLogLevel.eLogLevel_Info < Logger.s_eLogLevel)
            {
                return;
            }
            string text = string.Format("[Info] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
                return;
            }
            UnityEngine.Debug.Log(text);
        }
        public void Error(object message)
        {
            if (EnumLogLevel.eLogLevel_Error < Logger.s_eLogLevel)
            {
                return;
            }
            string text = string.Format("[Error] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
                return;
            }
            UnityEngine.Debug.LogError(text);
        }
        public void Fatal(object message)
        {
            if (EnumLogLevel.eLogLevel_Fatal < Logger.s_eLogLevel)
            {
                return;
            }
            string text = string.Format("[Fatal] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
                return;
            }
            UnityEngine.Debug.LogError(text);
        }
    }
}