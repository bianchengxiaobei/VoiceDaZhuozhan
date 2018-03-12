using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Threading;
using System.Diagnostics;
#region 模块信息
/*----------------------------------------------------------------
// 模块名：CNetLogger
// 创建者：chen
// 修改者列表：
// 创建日期：#CREATIONDATE#
// 模块描述：
//----------------------------------------------------------------*/
#endregion
namespace CaomaoFramework
{
    public class CNetLogger
    {
        private string m_strFilePath;
        private FileStream m_ofs = null;
        private StreamWriter m_owr = null;
        private int m_nLastSaveTime = 0;
        private bool m_bEnable = false;
        public CNetLogger(string strLogDir, bool bEnable)
        {
            this.m_bEnable = bEnable;
            if (this.m_bEnable)
            {
                try
                {
                    string text = "network.log";
                    if (strLogDir.Length == 0)
                    {
                        this.m_strFilePath = text;
                    }
                    else
                    {
                        if (strLogDir[strLogDir.Length - 1] == '\\' || strLogDir[strLogDir.Length - 1] == '/')
                        {
                            this.m_strFilePath = strLogDir + text;
                        }
                        else
                        {
                            this.m_strFilePath = strLogDir + "/" + text;
                        }
                    }
                    this.m_ofs = new FileStream(this.m_strFilePath, FileMode.Append, FileAccess.Write);
                    this.m_owr = new StreamWriter(this.m_ofs, Encoding.Unicode);
                    this.m_nLastSaveTime = Environment.TickCount;
                }
                catch (Exception ex)
                {
                    this.m_bEnable = false;
                    Trace.WriteLine(ex.Message);
                }
            }
        }
        public void Close()
        {
            if (this.m_bEnable)
            {
                Monitor.Enter(this.m_owr);
                this.Save();
                this.m_owr.Close();
                this.m_ofs.Close();
                Monitor.Exit(this.m_owr);
            }
        }
        public void LogError(string strLog)
        {
            if (this.m_bEnable)
            {
                Monitor.Enter(this.m_owr);
                int tickCount = Environment.TickCount;
                this.m_owr.WriteLine(string.Concat(new string[]
				{
					DateTime.Now.ToString(),
					" [ ",
					tickCount.ToString(),
					"] [Error]:   ",
					strLog
				}));
                this.Save();
                this.m_nLastSaveTime = tickCount;
                Monitor.Exit(this.m_owr);
            }
        }
        public void LogInfo(string strLog)
        {
            if (this.m_bEnable)
            {
                Monitor.Enter(this.m_owr);
                int tickCount = Environment.TickCount;
                this.m_owr.WriteLine(string.Concat(new string[]
				{
					DateTime.Now.ToString(),
					" [ ",
					tickCount.ToString(),
					"] [Info]:   ",
					strLog
				}));
                this.Save();
                this.m_nLastSaveTime = tickCount;
                Monitor.Exit(this.m_owr);
            }
        }
        public void Save()
        {
            if (this.m_bEnable)
            {
                try
                {
                    this.m_owr.Flush();
                    this.m_ofs.Flush();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }
    }
}