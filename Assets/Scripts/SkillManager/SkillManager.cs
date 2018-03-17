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
    public void LoadSkill()
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
        if (PlayerPrefs.HasKey("lockedskills"))
        {
            string[] lockedSkillcontent = PlayerPrefs.GetString("lockedskills").Split(',');
            foreach (var locked in lockedSkillcontent)
            {
                GameSkillBase skill = null;
                int skillId = int.Parse(locked);
                if (this.skills.TryGetValue(skillId, out skill))
                {
                    skill.bLocked = true;
                }
            }
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
    public void SlowDown(int skillId, bool allMonster)
    {
        if (skills.ContainsKey(skillId))
        {
            GameSkillBase skill = this.skills[skillId];
            int level = skill.level;
            if (level == 0)
            {
                Debug.LogError("level == 0");
                return ;
            }
            float slowValue = skill.skillConfig.param[level + 1].param;
            if (slowValue <= 0)
            {
                Debug.LogError("减速==0");
                return;
            }
            if (allMonster)
            {
                List<EntityParent> entitys = PlayerManager.singleton.allVisiableMonster;
                foreach (var moster in entitys)
                {
                    moster.ApplySlowDown((int)slowValue);
                }
            }
        }
    }
    public void Control(int skillId, bool allMonster)
    {
        if (skills.ContainsKey(skillId))
        {
            GameSkillBase skill = this.skills[skillId];
            int level = skill.level;
            if (level == 0)
            {
                Debug.LogError("level == 0");
                return;
            }
            float slowValue = skill.skillConfig.param[level + 1].param;
            if (slowValue <= 0)
            {
                Debug.LogError("减速==0");
                return;
            }
            if (allMonster)
            {
                List<EntityParent> entitys = PlayerManager.singleton.allVisiableMonster;
                foreach (var moster in entitys)
                {
                    moster.ApplySlowDown((int)slowValue);
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
    public GameSkillBase GetSkill(int skillId)
    {
        if (skills.ContainsKey(skillId))
        {
            return skills[skillId];
        }
        return null;
    }
}
