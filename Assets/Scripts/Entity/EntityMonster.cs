using UnityEngine;
using System.Collections.Generic;
using System;

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
        WWWResourceManager.Instance.Load(data.ModelFile, (assetRequest) =>
        {
            GameObject gameobject = assetRequest.Instantiate(false);
            gameobject.tag = "Player";
            GameObject.DontDestroyOnLoad(gameobject);
            if (fsmMotion == null)
            {
                fsmMotion = new FSMMotion();
            }
            animator = gameobject.GetComponent<Animator>();
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
            Transform.gameObject.layer = 8;
            GatherRenderNodeInfo(GameObject);
            this.SetVisiable(false);
            if (action != null)
            {
                action();
            }
        });
    }
}
