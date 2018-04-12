using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using Newtonsoft.Json;
using CaomaoFramework.Data;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：LevelEditor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LevelEditor : EditorWindow
{
    public static LevelConfig config;
    private ReorderableList list;
    private Vector2 srollPosition;
    private const string savePath = "Assets/LevelConfig/makeLevel.asset";
    [MenuItem("Window/关卡设计")]
    public static void Init()
    {
        LevelEditor myWindow = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor), false, "关卡设计", true);
        myWindow.Show();
    }
    public void OnGUI()
    {
        if (config == null)
        {
            InitConfig();
        }
        if (list == null)
        {
            InitLevelListGui();
        }
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("添加关卡", EditorStyles.toolbarButton))
            {
                this.AddLevel();
            }
            if (GUILayout.Button("保存设置", EditorStyles.toolbarButton))
            {
                this.SaveLevel();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("导出关卡", EditorStyles.toolbarButton))
            {
                this.ExportLevel();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("关卡导出格式");
                config.exprotFormat = (Format)EditorGUILayout.EnumPopup(config.exprotFormat);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("主角血量");
                config.SelfMaxHp = EditorGUILayout.IntField(config.SelfMaxHp);
                EditorGUILayout.PrefixLabel("纹身男血量");
                config.WenShenMaxHp = EditorGUILayout.IntField(config.WenShenMaxHp);
                EditorGUILayout.PrefixLabel("胖子男血量");
                config.PangziMaxHp = EditorGUILayout.IntField(config.PangziMaxHp);
                EditorGUILayout.PrefixLabel("飞机男血量");
                config.FeijiMaxHp = EditorGUILayout.IntField(config.FeijiMaxHp);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("跳跳男血量");
                config.TiaoTiaoMaxHp  = EditorGUILayout.IntField(config.TiaoTiaoMaxHp );
                EditorGUILayout.PrefixLabel("瞬移怪血量");
                config.ShunxiMaxHp  = EditorGUILayout.IntField(config.ShunxiMaxHp);
                EditorGUILayout.PrefixLabel("匍匐怪血量");
                config.PufuMaxHp  = EditorGUILayout.IntField(config.PufuMaxHp );
                EditorGUILayout.PrefixLabel("挖掘机血量");
                config.WajuejiMaxHp  = EditorGUILayout.IntField(config.WajuejiMaxHp);
            }
            GUILayout.EndHorizontal();
            srollPosition = GUILayout.BeginScrollView(srollPosition);
            {
                list.DoLayoutList();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(config);
        }

    }
    public void AddLevel()
    {
        var path = SelectFolder();
        if (!string.IsNullOrEmpty(path))
        {
            var level = new Level();
            level.levelPath = path;
            int index = config.levels.Count + 1;
            level.levelId = index;
            level.levelName = string.Format("关卡{0}", index);
            config.levels.Add(level);
        }
    }
    public void DeleteLevel(ReorderableList list)
    {
        config.levels.RemoveAt(list.index);
        this.Sort();
    }
    private void Sort()
    {
        int index = 0;
        foreach (var level in config.levels)
        {
            level.levelId = ++index;
            level.levelName = string.Format("关卡{0}", index);
        }
    }
    public void SaveLevel()
    {
        string pathName = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(pathName))
        {
            Directory.CreateDirectory(pathName);
        }
        if (AssetDatabase.LoadAssetAtPath<LevelConfig>(savePath) == null)
        {
            AssetDatabase.CreateAsset(config, savePath);
        }
        else
        {
            EditorUtility.SetDirty(config);
        }
    }
    public void InitLevelListGui()
    {
        list = new ReorderableList(config.levels, typeof(Level));
        list.drawElementCallback = this.LevelListDrawCallback;
        list.drawHeaderCallback = this.OnListHeaderGuiCallback;
        list.draggable = true;
        list.elementHeight = 22;
        list.onAddCallback = (list) => AddLevel();
        list.onRemoveCallback = (list) => DeleteLevel(list);
    }
    public void InitConfig()
    {
        config = AssetDatabase.LoadAssetAtPath<LevelConfig>(savePath);
        if (config == null)
        {
            config = new LevelConfig();
        }
    }
    public void LevelListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        Level level = config.levels[index];
        rect.y++;

        Rect r = rect;
        r.width = 16;
        r.height = 18;
        level.valid = GUI.Toggle(r, level.valid, GUIContent.none);

        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        GUI.enabled = false;
        level.levelId = EditorGUI.IntField(r, "关卡Id", level.levelId);
        GUI.enabled = true;

        r.xMin = r.xMax + GAP;
        r.width = 100;
        if (GUI.Button(r, "选择保存路径"))
        {
            var path = SelectFolder();
            if (path != null)
            {
                level.levelPath = path;
            }
        }
        r.xMin = r.xMax + GAP;
        r.width = 50;
        if (GUI.Button(r, "编辑"))
        {
            LevelDetailEditor.OpenWindow(level);
        }
        r.xMin = r.xMax + GAP;
        r.xMax = rect.xMax;
        level.levelName = GUI.TextField(r, level.levelName);
    }
    public void OnListHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "关卡");
    }
    public void ExportLevel()
    {
        if (config.exprotFormat == Format.Json)
        {
            string json = JsonConvert.SerializeObject(config);
            if (!string.IsNullOrEmpty(json))
            {
                XMLParser.SaveText(Application.dataPath + "/Resources/levels.txt", json);
            }
        }
    }
    string SelectFolder()
    {
        string dataPath = Application.dataPath;
        string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(dataPath))
            {
                return "Assets/" + selectedPath.Substring(dataPath.Length + 1);
            }
            else
            {
                ShowNotification(new GUIContent("不能在Assets目录之外!"));
            }
        }
        return null;
    }
}
