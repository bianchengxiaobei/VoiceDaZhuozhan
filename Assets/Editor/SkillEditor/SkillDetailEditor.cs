using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
/*----------------------------------------------------------------
// 模块名：SkillDetailEditor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class SkillDetailEditor : EditorWindow
{
    private static Skill m_oSkill;
    private Vector2 cdsrollPosition;
    private ReorderableList cdlist;
    private Vector2 demagesrollPosition;
    private ReorderableList demagelist;
    private Vector2 paramsrollPosition;
    private ReorderableList paramlist;
    private string audioPath;
    private string levelGold;
    public static void OpenWindow(Skill skill)
    {
        m_oSkill = skill;
        SkillDetailEditor myWindow = (SkillDetailEditor)EditorWindow.GetWindow(typeof(SkillDetailEditor), false, "技能编辑", true);
        myWindow.Show();
    }
    public void OnGUI()
    {
        if (m_oSkill == null)
        {
            return;
        }
        if (cdlist == null)
        {
            this.InitCDListGui();
        }
        if (demagelist == null)
        {
            this.InitDemageListGui();
        }
        if (paramlist == null)
        {
            this.InitParamListGui();
        }
        this.ShowDetailInfo();
        GUILayout.BeginVertical();
        {
            GUILayout.Space(10);
            cdsrollPosition = GUILayout.BeginScrollView(cdsrollPosition);
            {
                cdlist.DoLayoutList();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        {
            GUILayout.Space(10);
            demagesrollPosition = GUILayout.BeginScrollView(demagesrollPosition);
            {
                demagelist.DoLayoutList();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }
    public void InitCDListGui()
    {
        cdlist = new ReorderableList(m_oSkill.cds, typeof(CD));
        cdlist.drawElementCallback = this.CDListDrawCallback;
        cdlist.drawHeaderCallback = this.OnCDHeaderGuiCallback;
        cdlist.draggable = true;
        cdlist.elementHeight = 22;
        cdlist.onAddCallback = (list) => AddCD();
    }
    public void InitDemageListGui()
    {
        demagelist = new ReorderableList(m_oSkill.demage, typeof(Demage));
        demagelist.drawElementCallback = this.DemageListDrawCallback;
        demagelist.drawHeaderCallback = this.OnDemageHeaderGuiCallback;
        demagelist.draggable = true;
        demagelist.elementHeight = 22;
        demagelist.onAddCallback = (list) => AddDemage();
    }
    public void InitParamListGui()
    {
        paramlist = new ReorderableList(m_oSkill.param, typeof(Param));
        paramlist.drawElementCallback = this.ParamListDrawCallback;
        paramlist.drawHeaderCallback = this.OnParamHeaderGuiCallback;
        paramlist.draggable = true;
        paramlist.elementHeight = 22;
        paramlist.onAddCallback = (list) => AddParam();
    }
    public void CDListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        CD cd = m_oSkill.cds[index];
        rect.y++;
        Rect r = rect;
        r.width = 200;
        r.height = 18;
        cd.level = EditorGUI.IntField(r, "等级", cd.level);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        cd.cd = EditorGUI.FloatField(r, "CD", cd.cd);
    }
    public void OnCDHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "CD编辑");
    }
    public void OnDemageHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "伤害编辑");
    }
    public void DemageListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        Demage demage = m_oSkill.demage[index];
        rect.y++;
        Rect r = rect;
        r.width = 200;
        r.height = 18;
        demage.level = EditorGUI.IntField(r, "等级", demage.level);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        demage.demage = EditorGUI.FloatField(r, "伤害", demage.demage);
    }
    public void OnParamHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "参数编辑");
    }
    public void ParamListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        Param param = m_oSkill.param[index];
        rect.y++;
        Rect r = rect;
        r.width = 200;
        r.height = 18;
        param.name = EditorGUI.TextField(r, "参数名字", param.name);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        param.param = EditorGUI.FloatField(r, "参数数值", param.param);
    }

    public void AddCD()
    {
        CD cd = new CD();
        if (m_oSkill != null)
        {
            int index = m_oSkill.cds.Count;
            cd.level = ++index;
            m_oSkill.cds.Add(cd);
        }
    }
    public void AddDemage()
    {
        Demage demage = new Demage();
        if (m_oSkill != null)
        {
            int index = m_oSkill.cds.Count;
            demage.level = ++index;
            m_oSkill.demage.Add(demage);
        }
    }
    public void AddParam()
    {
        Param param = new Param();
        if (m_oSkill != null)
        {
            m_oSkill.param.Add(param);
        }
    }
    public void ShowDetailInfo()
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能ID");
            m_oSkill.skillId = EditorGUILayout.IntField(m_oSkill.skillId);
            m_oSkill.skillName = EditorGUILayout.TextField("技能名称",m_oSkill.skillName);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能口令");
            m_oSkill.skillToken = EditorGUILayout.TextField(m_oSkill.skillToken);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能描述");
            m_oSkill.skillInfo = EditorGUILayout.TextArea(m_oSkill.skillInfo, GUILayout.MaxHeight(75));
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能图标名字");
            m_oSkill.iconPath = EditorGUILayout.TextField(m_oSkill.iconPath);
            if (string.IsNullOrEmpty(m_oSkill.iconPath))
            {
                m_oSkill.iconPath = m_oSkill.skillName;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能动作名称");
            m_oSkill.animation = EditorGUILayout.TextField(m_oSkill.animation);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能特效路径");
            m_oSkill.effectPath = EditorGUILayout.TextField(m_oSkill.effectPath);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能音频名称");
            m_oSkill.audioPath = "Assets.Audios.SkillEffects."+EditorGUILayout.TextField(audioPath);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能解锁金币");
            m_oSkill.lockGold = EditorGUILayout.IntField(m_oSkill.lockGold);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("技能升级金币");
            this.levelGold = EditorGUILayout.TextField(levelGold);
            string[] content = this.levelGold.Split(',');
            m_oSkill.upgradeGold.Clear();
            if (content != null && content.Length > 0)
            {
                foreach (var gold in content)
                {
                    m_oSkill.upgradeGold.Add(int.Parse(gold));
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        this.ShowShapeType();
    }
    private void ShowShapeType()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("技能形状类型");
                m_oSkill.type = (ESkillType)EditorGUILayout.EnumPopup(m_oSkill.type);
            }
            GUILayout.EndHorizontal();
            if (m_oSkill.type == ESkillType.直线有范围 || m_oSkill.type == ESkillType.固定区域 || m_oSkill.type == ESkillType.全屏 || m_oSkill.type == ESkillType.锁定)
            {
                GUILayout.Space(10);
                paramsrollPosition = GUILayout.BeginScrollView(paramsrollPosition);
                {
                    paramlist.DoLayoutList();
                }
                GUILayout.EndScrollView();
            }
            if (m_oSkill.type == ESkillType.直线)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("技能特效生成位置");
                    m_oSkill.effectPos = (EffectBindPos)EditorGUILayout.EnumPopup(m_oSkill.effectPos);
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }
}
