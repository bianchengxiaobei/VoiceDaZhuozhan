using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaomaoFramework
{
    public class CommonDefineBase
    {
        //public static readonly string VersionPath = Application.persistentDataPath + "/version.xml";
        //public static readonly string ServerVersionPath = Application.persistentDataPath + "/server_version.xml";
        public static string ResourcePath = Application.persistentDataPath + "/Assetbundle";    
    }
    public enum GraphicsQuality
    {
        Low,
        Medium,
        High
    }
    public enum EffectLodLevel
    {
        High,
        Low
    }
    public enum EBattleServerState
    {
        eSSBS_SelectHero,
        eSSBS_SelectRune,
        eSSBS_Prepare,
        eSSBS_Loading,
        eSSBS_Playing,
        eSSBS_Finished
    }
    public enum ECampType
    {
        eAllEnemy = -1,
        eAllPeace,
        eCamp1,
        eCamp2,
        eCamp3,
        eCamp4,
        eCamp5,
        eCamp6
    }
    /// <summary>
    /// 新手引导总的分几个大步骤
    /// </summary>
    public enum EGuideStepInfo
    {
        GuideStepNull = 0,
        PrimaryGuide = 1001,
        BuyHeroGuide,
        BattleAiGuide
    }
    /// <summary>
    /// 任务检查类型
    /// </summary>
    public enum TaskCheckType
    {
        AllMeetCheck = 1,
        PartMeetCheck,
        AnyMeetCheck
    }
    public enum EGuideTaskType
    {
        ClickButtonTask = 1,
        ForceClickTask = 2,
        TipTask = 3,
        MoveCameraTask = 4,
        PopTipWindowTask = 5,
        PathTask = 6,
        StartGameMode = 7,
        ShowTipTask = 8,
        GetHeroTask = 9,
        SelectableTask = 10
    }
    public enum ESkillShapeType
    {
        OuterCircle,    // 外圆
        InnerCircle,    // 内圆
        Cube,           // 矩形 
        Sector60,        // 扇形
        Sector120,        // 扇形
        NormalOuterCircle,//普攻外圈
        None
    }
    public enum ESkillAreaType
    {
        OuterCircle = 0,
        OuterCircle_InnerCube = 1,
        OuterCircle_InnerSector = 2,
        OuterCircle_InnerCircle = 3,
        NormalOuterCircle = 4
    }
    /// <summary>
    /// 认证类型
    /// </summary>
    public enum EAccountAuthType : byte
    {
        ACCOUNT_AUTH_NORMAL = 0,
        ACCOUNT_AUTH_INTERNAL,
        ACCOUNT_AUTH_GUEST
    }
    /// <summary>
    /// 漂浮类型
    /// </summary>
    public enum EFlyTextType
    {
        eFlyTextType_Common,
        eFlyTextType_Hp,
        eFlyTextType_SystemInfo,
        eFlyTextType_Report,
        eFlyTextType_Kill,
        eFlyTextType_Invite,
        eFlyTextType_Max
    }
    /// <summary>
    /// 扣血类型
    /// </summary>
    public enum EHpEffectType
    {
        eHpEffectType_Damage,
        eHpEffectType_Heal
    }
    /// <summary>
    /// 服务器向导步骤类型
    /// </summary>
    public enum EGuideStepType : byte
    {
        eStartGameMode = 0
    }
    /// <summary>
    /// 购买方式
    /// </summary>
    public enum EBuyType : byte
    {
        Gold = 0,
        Diamond
    }
    /// <summary>
    /// 好友关系
    /// </summary>
    public enum ERelativeShip
    {
        Friend,
        Black,
        NoneFriend
    }
    /// <summary>
    /// 好友状态
    /// </summary>
    public enum EFriendStatus : byte
    {
        Online = 1,
        InBattle = 2,
        Offline = 0
    }
    /// <summary>
    /// 角色状态
    /// </summary>
    public enum EPlayerStatus : byte
    {
        Normal,
        魔法护盾,
        减速,
        眩晕
    }
    public enum EntityType
    {
        主角 = 1,
        纹身男,
        胖子男,
        跳跳男,
        飞机男,
        挖掘机,
        瞬移怪,
        匍匐怪
    }
    public enum Format
    {
        Bin,
        Xml,
        Json
    }
}
