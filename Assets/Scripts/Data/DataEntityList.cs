using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：DataEntityList
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class DataEntityList : GameData<DataEntityList>
{
    public static string fileName = "DataEntityList";
    public string ModelFile { get; set; }
    public string EntityInfo { get; set; }
    public string Name { get; set; }
}
