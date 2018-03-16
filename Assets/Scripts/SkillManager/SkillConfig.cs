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
public class SkillConfig
{
    public List<Skill> skills = new List<Skill>();
}
[System.Serializable]
public class Skill
{
    public int skillId;
    public string skillName;
    public string skillInfo;
    public string skillToken;//口令
    public ESkillType type;
    public List<Param> param = new List<Param>();//参数
    public List<CD> cds = new List<CD>();//根据等级
    public List<Demage> demage = new List<Demage>();//根据等级
    public string animation;//动画
    public string effectPath;//特效路劲
    public EffectBindPos effectPos;//特效出生位置
    public string iconPath;//技能icon名称
}
public enum ESkillType
{
    全屏 = 1,
    直线,
    直线有范围,
    固定区域,
    选择方向,
    选择固定位置,
    锁定
}
public enum EffectBindPos
{
    左手,
    右手,
    中心,
    左脚,
    右脚,
    枪口
}
[System.Serializable]
public class CD
{
    public int level;
    public float cd;
}
[System.Serializable]
public class Demage
{
    public int level;
    public float demage;
}
[System.Serializable]
public class Param
{
    public string name;
    public int level;
    public float param;
}
