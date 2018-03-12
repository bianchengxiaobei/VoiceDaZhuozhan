using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using Newtonsoft.Json;
/*----------------------------------------------------------------
// 模块名：SkillManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    public Dictionary<int, GameSkillBase> skills = new Dictionary<int, GameSkillBase>();
    public Dictionary<string, GameSkillBase> skillStrings = new Dictionary<string, GameSkillBase>();
    public void LoadSkill(string filePath)
    {
        string content = UnityTool.LoadTxtFile(Application.dataPath + "/Resources/skills.txt");
        SkillConfig config = JsonConvert.DeserializeObject<SkillConfig>(content);
        foreach (var skill in config.skills)
        {
            GameSkillBase gameSkill = this.MakeSkill(skill);
            if (skill == null)
            {
                Debug.LogError("skill == null");
                continue;
            }
            gameSkill.Init(skill);
            this.skills.Add(skill.skillId, gameSkill);
            this.skillStrings.Add(skill.skillToken, gameSkill);
        }
    }
    public void Attack(int skillId,bool allMonster)
    {
        if (skills.ContainsKey(skillId))
        {
            float demage = this.skills[skillId].skillConfig.demage[0].demage;
            if (demage == 0)
            {
                Debug.LogError("伤害==0");
                return;
            }
            if (allMonster)
            {
                List<EntityParent> entitys = PlayerManager.singleton.allVisiableMonster;
                foreach (var moster in entitys)
                {
                    moster.OnHit();
                    moster.ApplyDemage(demage);
                }
            }         
        }
    }
    public GameSkillBase MakeSkill(Skill skill)
    {
        switch (skill.type)
        {
            case ESkillType.全屏:
                return new QuanPingSkill();
        }
        return null;
    }
    public GameSkillBase GetSkill(string token)
    {
        if (skillStrings.ContainsKey(token))
        {
            return skillStrings[token];
        }
        return null;
    }
}
