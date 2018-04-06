using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using CaomaoFramework.EntityFsm;
using System;
/*----------------------------------------------------------------
// 模块名：StateDead
// 创建者：chen
// 修改者列表：
// 创建日期：2017.7.9
// 模块描述：死亡状态
//--------------------------------------------------------------*/
/// <summary>
/// 死亡状态
/// </summary>
public class StateDead : IEntityState
{
    public _2dxFX_DesintegrationFX deadFx;
    private float time = 0;
    public void Enter(EntityParent entity, params object[] args)
    {
        entity.CurrentMotionState = MotionState.DEAD;
    }

    public void Exit(EntityParent entity, params object[] args)
    {
        time = 0;   
    }
    public void Process(EntityParent entity, params object[] args)
    {
        deadFx = entity.Transform.GetComponent<_2dxFX_DesintegrationFX>();
        if (deadFx != null)
        {
            deadFx.ForceMaterial = entity.Transform.GetComponent<MeshRenderer>().material;
            deadFx.enabled = true;
        }
        entity.animator.CrossFade("Dead", 0.0f);
        entity.bIsDead = true;
        entity.entityActor.moveDir = Vector3.zero;
        entity.LeaveLevel();
        DataEntityList data = DataEntityList.dataMap[entity.type];
        AudioType type = AudioType.纹身男死亡;
        switch (data.Name)
        {
            case "纹身男":
                type = AudioType.纹身男死亡;
                break;
            case "胖子男":
                type = AudioType.胖子男死亡;
                break;
        }
        //播放死亡音效
        AudioManagerBase.Instance.LogicPlaySoundByClip(entity.audioSource, ResourcePoolManager.singleton.GetAudioClip(type));
    }
    public void Execute(EntityParent theOwner)
    {
        if (deadFx != null)
        {
            time += Time.deltaTime * 0.5f;
            deadFx.Desintegration = Mathf.Lerp(0, 1, time);
        }
    }
}
