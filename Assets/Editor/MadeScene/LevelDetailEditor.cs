using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
/*----------------------------------------------------------------
// 模块名：LevelDetailEditor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LevelDetailEditor : EditorWindow
{
    private static Level m_oLevel;
    private Vector2 srollPosition;
    private ReorderableList list;
    public static void OpenWindow(Level level)
    {
        m_oLevel = level;
        LevelDetailEditor myWindow = (LevelDetailEditor)EditorWindow.GetWindow(typeof(LevelDetailEditor), false, "关卡编辑", true);
        myWindow.Show();
    }
    public void OnGUI()
    {
        if (m_oLevel == null)
        {
            return;
        }
        if (list == null)
        {
            this.InitLevelListGui();
        }
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
        list = new ReorderableList(m_oLevel.waves, typeof(Wave));
        list.drawElementCallback = this.LevelListDrawCallback;
        list.drawHeaderCallback = this.OnListHeaderGuiCallback;
        list.draggable = true;
        list.elementHeight = 22;
        list.onAddCallback = (list) => AddWave();
    }
    public void LevelListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        Wave wave = m_oLevel.waves[index];
        rect.y++;
        Rect r = rect;
        r.width = 50;
        r.height = 18;
        GUI.Label(r, wave.name);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 80;
        GUI.Label(r, "持续时间");
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 100;
        wave.duration = GUI.HorizontalSlider(r, wave.duration, 10, 300);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 100;
        GUI.Label(r, wave.duration.ToString());
        r.xMin = r.xMax + GAP;
        r.width = 100;
        if (GUI.Button(r, "添加怪物"))
        {
            MasterEditor.OpenWindow(wave);
        }
    }
    public void OnListHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "怪物波数");
    }
    public void AddWave()
    {
        Wave wave = new Wave();
        if (m_oLevel != null)
        {
            int index = m_oLevel.waves.Count;
            wave.index = ++index;
            wave.name = string.Format("波数{0}", wave.index);
            m_oLevel.waves.Add(wave);
        }
    }
}

