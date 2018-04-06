using UnityEngine;
using System.Collections.Generic;
using System;
using CaomaoFramework;
/*----------------------------------------------------------------
// 模块名：EntityMonster
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class EntityMonster : EntityParent
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
            Renderer = gameobject.GetComponentInChildren<Renderer>();
            entityActor = gameobject.GetComponent<EntityActor>();
            entityActor.entity = this;
            entityActor.Id = this.EntityId;
            Transform.gameObject.layer = LayerMask.NameToLayer("Enemy");
            GatherRenderNodeInfo(GameObject);
            this.collider2d.enabled = true;
            this.entityActor.enabled = true;
            bIsDead = false;
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
                Renderer = gameobject.GetComponentInChildren<Renderer>();
                entityActor = gameobject.GetComponent<EntityActor>();
                entityActor.entity = this;
                entityActor.Id = this.EntityId;
                Transform.gameObject.layer = LayerMask.NameToLayer("Enemy");
                GatherRenderNodeInfo(GameObject);
                this.SetVisiable(false);
                if (action != null)
                {
                    action();
                }
            });
        }
    }
    public override void GatherRenderNodeInfo(GameObject rootRenderNode)
    {
        bindNodeTable[CommonDefineBase.BindCenter] = rootRenderNode.transform.Find(CommonDefineBase.BindCenter);
        bindNodeTable[CommonDefineBase.BindRightFoot] = rootRenderNode.transform.Find(CommonDefineBase.BindRightFoot);
    }
}
