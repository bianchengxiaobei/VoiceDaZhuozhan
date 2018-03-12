using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：MasterEditor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class MasterEditor : EditorWindow
{
    public static Wave m_oWave;
    private Vector2 srollPosition;
    private ReorderableList list;
    public static void OpenWindow(Wave wave)
    {
        m_oWave = wave;
        MasterEditor myWindow = (MasterEditor)EditorWindow.GetWindow(typeof(MasterEditor), false, "怪物编辑", true);
        myWindow.Show();
    }
    private void OnGUI()
    {
        if (m_oWave == null)
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
        list = new ReorderableList(m_oWave.entitys, typeof(MasterWave));
        list.drawElementCallback = this.LevelListDrawCallback;
        list.drawHeaderCallback = this.OnListHeaderGuiCallback;
        list.draggable = true;
        list.elementHeight = 22;
        list.onAddCallback = (list) => AddMasterWave();
    }
    public void LevelListDrawCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;
        MasterWave master = m_oWave.entitys[index];
        rect.y++;
        Rect r = rect;
        r.width = 200;
        r.height = 18;
        master.type = (EntityType)EditorGUI.EnumPopup(r,"怪物类型",master.type);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        master.speed = EditorGUI.FloatField(r, "怪物移动速度",master.speed);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        master.duration = EditorGUI.FloatField(r, "下个怪物生成时间", master.duration);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        master.num = EditorGUI.IntField(r, "数量", master.num);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        master.maxHp = EditorGUI.IntField(r, "血量", master.maxHp);
        r.xMin = r.xMax + GAP;
        r.xMax = r.xMax + 200;
        master.demage = EditorGUI.IntField(r, "攻击伤害", master.demage);
    }
    public void OnListHeaderGuiCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "怪物");
    }
    public void AddMasterWave()
    {
        MasterWave master = new MasterWave();
        m_oWave.entitys.Add(master);
    }
}
