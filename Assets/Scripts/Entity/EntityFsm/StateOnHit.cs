using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;

public class StateOnHit : IEntityState
{
    public GameObject theHitEffect;
    private Vector3 oldMove;
    public void Enter(EntityParent entity, params object[] args)
    {
        entity.CurrentMotionState = MotionState.HIT;
    }

    public void Execute(EntityParent entity)
    {
        
    }

    public void Exit(EntityParent entity, params object[] args)
    {
        entity.entityActor.moveDir = oldMove;
        this.theHitEffect = null;
    }

    public void Process(EntityParent entity, params object[] args)
    {
        if (entity is EntityMyself && SkillManager.singleton.HasSkillRunning())
        {
            
        }
        else
        {
            entity.PlayAnimation("OnHit");
        }       
        oldMove = entity.entityActor.moveDir;
        entity.entityActor.moveDir = Vector3.zero;
        //受击特效
        Skill skill = args[0] as Skill;
        if (skill != null || (skill == null && entity is EntityMyself))
        {
            string path = string.Empty;
            if (skill == null)
            {
                path = "Assets.Prefabs.Effects.Hit.HitBai.prefab";
            }
            else
            {
                switch (skill.hitType)
                {
                    case OnHitEffectType.白色:
                        path = "Assets.Prefabs.Effects.Hit.HitBai.prefab";
                        break;
                    case OnHitEffectType.橙色:
                        path = "Assets.Prefabs.Effects.Hit.HitCheng.prefab";
                        break;
                    case OnHitEffectType.紫色:
                        path = "Assets.Prefabs.Effects.Hit.HitZi.prefab";
                        break;
                    case OnHitEffectType.红色:
                        path = "Assets.Prefabs.Effects.Hit.HitHong.prefab";
                        break;
                }
            }
            WWWResourceManager.Instance.Load(path, (asset) => 
            {
                if (asset != null)
                {
                    theHitEffect = asset.Instantiate();
                    this.theHitEffect.transform.SetParent(entity.bindNodeTable[CommonDefineBase.BindCenter] as Transform);
                    theHitEffect.transform.localPosition = Vector3.zero;
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
                    //播放音效
                    AudioManagerBase.Instance.LogicPlaySoundByClip(entity.audioSource, ResourcePoolManager.singleton.GetAudioClip(type));
                }
            });
            if (entity is EntityMonster)
            {
                entity.timer.Add(TimerManager.AddTimer(500, 0, () =>
                {
                    entity.Move();
                }));
            }
            else
            {
                entity.Idle();
            }
        }
    }
}
