using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：SkillConfig
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
[System.Serializable]
public class SkillConfig : ScriptableObject
{
    public List<Skill> skills = new List<Skill>();	
}

