using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
/*----------------------------------------------------------------
// 模块名：SkillEditor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class SkillEditor : EditorWindow
{
    private SkillConfig config;
    private Vector2 srollPosition;
    private ReorderableList list;
    private const string savePath = "Assets/SkillConfig/makeskill.asset";
    [MenuItem("Window/技能编辑")]
    public static void Init()
    {
        SkillEditor myWindow = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor), false, "技能编辑", true);
        myWindow.Show();
    }
    private void OnGUI()
    {
        if (config == null)
        {
            InitConfig();
        }
        if (list == null)
        {
            this.InitLevelListGui();
        }
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("保存设置", EditorStyles.toolbarButton))
            {
                this.SaveSkill();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("导出技能", EditorStyles.toolbarButton))
            {
                
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        {
            GUILayout.Space(10);
            srollPosition = GUILayout.BeginScrollView(srollPosition);
            {
                list.DoLayoutList();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }
    public void InitLevelListGui()
    {
        list = new ReorderableList(config.skills, typeof(Skill));
        list.drawElementCallback = this.LevelListDrawCallback;
        list.drawHeaderCallback = this.OnListHeaderGuiCallback;
        list.draggable = true;
        list.elementHeight = 22;
        list.onAddCallback = (list) => AddSkill();
    }
    public void InitConfig()
    {
        config = AssetDatabase.LoadAssetAtPath<SkillConfig>(savePath);
        if (config == null)
        {
            config = new SkillConfig();
        }
    }
    public void LevelListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;
        Skill skill = config.skills[index];
        rect.y++;
        Rect r = rect;
        r.width = 200;
        r.height = 18;
        skill.skillId = EditorGUI.IntField(r, "技能ID", skill.skillId);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 400;
        skill.skillName = EditorGUI.TextField(r, "技能名称", skill.skillName);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 40;
        if (GUI.Button(r, "编辑"))
        {
            SkillDetailEditor.OpenWindow(skill);
        }
    }
    public void OnListHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "技能");
    }
    public void AddSkill()
    {
        Skill skill = new Skill();
        this.config.skills.Add(skill);
    }
    public void SaveSkill()
    {
        string pathName = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(pathName))
        {
            Directory.CreateDirectory(pathName);
        }
        if (AssetDatabase.LoadAssetAtPath<SkillConfig>(savePath) == null)
        {
            AssetDatabase.CreateAsset(config, savePath);
        }
        else
        {
            EditorUtility.SetDirty(config);
        }
    }
}
