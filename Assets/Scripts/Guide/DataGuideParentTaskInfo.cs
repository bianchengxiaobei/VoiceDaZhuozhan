using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：DataGuideParentTaskInfo
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataGuideParentTaskInfo : GameData<DataGuideParentTaskInfo>
{
    public static string fileName = "DataGuideParentTaskInfo";
    public List<int> ChildTaskType { get; set; }
    public List<int> ChildTaskId { get; set; }
    public int EndTaskChildId { get; set; }
    public int NextTaskId { get; set; }
    public int TaskType { get; set; }
    public bool Moduleend { get; set; }//是否是模块的最后一个任务,告诉服务器
}
