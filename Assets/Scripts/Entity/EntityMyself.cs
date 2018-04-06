using System;
using UnityEngine;
using CaomaoFramework;
public class EntityMyself : EntityParent
{
    public override void CreateEntity(Action action)
    {
        DataEntityList data = DataEntityList.dataMap[this.type];
        if (ResourcePoolManager.singleton.dicEffectPool.ContainsKey(data.ModelFile))
        {
            Transform prefab = ResourcePoolManager.singleton.dicEffectPool[data.ModelFile];
            GameObject gameobject = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(prefab).gameObject;
            gameobject.tag = "Player";
            GameObject.DontDestroyOnLoad(gameobject);
            if (fsmMotion == null)
            {
                fsmMotion = new FSMMotion();
            }
            bIsDead = false;
            animator = gameobject.GetComponent<Animator>();
            this.collider2d = gameobject.GetComponent<Collider2D>();
            audioSource = gameobject.GetComponent<AudioSource>();
            if (null == audioSource)
            {
                audioSource = gameobject.AddComponent<AudioSource>();
            }
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            GameObject = gameobject;
            Transform = gameobject.transform;
            entityActor = GameObject.GetComponent<EntityActor>();
            if (entityActor == null)
            {
                entityActor = GameObject.AddComponent<EntityActor>();
            }
            entityActor.entity = this;
            collider2d.enabled = true;
            entityActor.enabled = true;
            Renderer = gameobject.GetComponentInChildren<Renderer>();
            Transform.gameObject.layer = LayerMask.NameToLayer("Player");
            GatherRenderNodeInfo(GameObject);
            this.SetVisiable(false);
            if (action != null)
            {
                action();
            }
        }
        else
        {
            WWWResourceManager.Instance.Load(data.ModelFile, (assetRequest) =>
            {
                Transform assetTra = (assetRequest.mainObject as GameObject).transform;
                GameObject gameobject = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
                ResourcePoolManager.singleton.dicEffectPool.Add(data.ModelFile, assetTra);
                gameobject.tag = "Player";
                GameObject.DontDestroyOnLoad(gameobject);
                if (fsmMotion == null)
                {
                    fsmMotion = new FSMMotion();
                }
                animator = gameobject.GetComponent<Animator>();
                this.collider2d = gameobject.GetComponent<Collider2D>();
                audioSource = gameobject.GetComponent<AudioSource>();
                if (null == audioSource)
                {
                    audioSource = gameobject.AddComponent<AudioSource>();
                }
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                GameObject = gameobject;
                Transform = gameobject.transform;
                entityActor = GameObject.GetComponent<EntityActor>();
                if (entityActor == null)
                {
                    entityActor = GameObject.AddComponent<EntityActor>();
                }
                entityActor.entity = this;
                Renderer = gameobject.GetComponentInChildren<Renderer>();
                Transform.gameObject.layer = LayerMask.NameToLayer("Player");
                GatherRenderNodeInfo(GameObject);
                this.SetVisiable(false);
                if (action != null)
                {
                    action();
                }
            });
        }     
    }
    public void CastSkill(string skillToken)
    {
        if (string.IsNullOrEmpty(skillToken))
        {
            return;
        }
        int skillId = SkillManager.singleton.MatchSkill(skillToken);
        if (skillId == -1)
        {
            Debug.Log("匹配不到技能");
            return;
        }
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillId);
        if (skill != null)
        {
            this.ChangeMotionState(MotionState.ReleaseSkill,skill);
        }
    }
    public override void GatherRenderNodeInfo(GameObject rootRenderNode)
    {
        bindNodeTable[CommonDefineBase.BindCenter] = rootRenderNode.transform.Find(CommonDefineBase.BindCenter);
        bindNodeTable[CommonDefineBase.BindRightHand] = rootRenderNode.transform.Find(CommonDefineBase.BindRightHand);
        bindNodeTable[CommonDefineBase.BindQiangkou] = rootRenderNode.transform.Find(CommonDefineBase.BindQiangkou);
        bindNodeTable[CommonDefineBase.BindRightFoot] = rootRenderNode.transform.Find(CommonDefineBase.BindRightFoot);
    }
}

