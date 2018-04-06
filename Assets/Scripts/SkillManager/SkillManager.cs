using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
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
    public Dictionary<int, Regex> skillRegexs = new Dictionary<int, Regex>();
    public List<GameSkillBase> runningSkills = new List<GameSkillBase>();
    public HashSet<GameSkillBase> lockedSkills = new HashSet<GameSkillBase>();
    public void LoadSkill()
    {
        string content = (Resources.Load("skills") as TextAsset).text;
        if (string.IsNullOrEmpty(content))
        {
            Debug.LogError("SkillContent == null");
            return;
        }
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
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < skill.skillToken.Length; i++)
            {
                sb.Append(CommonDefineBase.RegexFormat);
                sb.Append(skill.skillToken[i]);
                if (i == skill.skillToken.Length - 1)
                {
                    sb.Append(CommonDefineBase.RegexFormat);
                }
            }
            Regex reg = new Regex(sb.ToString());
            this.skillRegexs.Add(skill.skillId,reg);
        }
        if (PlayerPrefs.HasKey(CommonDefineBase.LockedSkills))
        {
            if (!GuideModel.singleton.bIsGuideAllComp)
            {
                PlayerPrefs.DeleteKey(CommonDefineBase.LockedSkills);
                return;
            }
            string[] lockedSkillcontent = PlayerPrefs.GetString(CommonDefineBase.LockedSkills).Split(',');
            foreach (var locked in lockedSkillcontent)
            {
                GameSkillBase skill = null;
                int skillId = int.Parse(locked);
                if (this.skills.TryGetValue(skillId, out skill))
                {
                    skill.bLocked = true;
                    this.lockedSkills.Add(skill);
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
                List<EntityParent> entitys = new List<EntityParent>(PlayerManager.singleton.allVisiableMonster);
                for (int i=0;i<entitys.Count;i++)
                {
                    entitys[i].ApplyDemage(demage, this.skills[skillId].skillConfig);
                }
            }  
        }
    }
    public void SlowDown(int skillId, bool allMonster,float duration)
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
            float slowValue = skill.skillConfig.param[level + 2].param;
            if (slowValue <= 0)
            {
                Debug.LogError("减速==0");
                return;
            }
            if (allMonster)
            {
                List<EntityParent> entitys = new List<EntityParent>(PlayerManager.singleton.allVisiableMonster);
                foreach (var moster in entitys)
                {
                    moster.ApplySlowDown((int)slowValue,duration);
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
            float controllValue = skill.skillConfig.param[level + 1].param;
            if (controllValue <= 0)
            {
                Debug.LogError("减速==0");
                return;
            }
            if (allMonster)
            {
                List<EntityParent> entitys = PlayerManager.singleton.allVisiableMonster;
                foreach (var moster in entitys)
                {
                    moster.ApplyControl(controllValue);
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
            case ESkillType.直线:
                return new LineSkill();
            case ESkillType.直线有范围:
                return new LineRangeSkill();
            case ESkillType.锁定:
                return new LockSkill();
            case ESkillType.选择方向:
                return new SelectDirSkill();
            case ESkillType.选择固定位置:
                return new SelectFixAreaSkill();
            case ESkillType.固定区域:
                return new FixAreaSkill();
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
    public bool LockSkill(int skillId)
    {
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillId);
        if (skill != null)
        {
            skill.bLocked = true;
            this.lockedSkills.Add(skill);
            if (!PlayerPrefs.HasKey(CommonDefineBase.LockedSkills))
            {                
                PlayerPrefs.SetString(CommonDefineBase.LockedSkills, skillId.ToString());
            }
            else
            {
                string content = PlayerPrefs.GetString(CommonDefineBase.LockedSkills);
                content += "," + skillId;
                PlayerPrefs.SetString(CommonDefineBase.LockedSkills, content);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public int MatchSkill(string token)
    {
        foreach (var entry in this.skillRegexs)
        {
            if (entry.Value.IsMatch(token))
            {
                return entry.Key;
            }
        }
        return -1;
    }
    public void Update()
    {
        for (int i = 0; i < this.runningSkills.Count; i++)
        {
            this.runningSkills[i].OnUpdate();
        }
    }
    public void ClearSkill()
    {
        for (int i = 0; i < this.runningSkills.Count; i++)
        {
            Debug.Log("fefwe:" + this.runningSkills[i].skillConfig.skillName);
            this.runningSkills[i].Exit();
        }
    }
    public bool IsSkillInRunning(int skillId)
    {
        foreach (var skill in this.runningSkills)
        {
            if (skill.skillConfig.skillId == skillId)
            {
                return true;
            }
            GameSkillBase skill1 = this.GetSkill(skillId);
            if (skill.skillConfig.HasActor == false)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasSkillRunning()
    {
        return this.runningSkills.Count > 0;
    }
}
