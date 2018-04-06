using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class EntityLearn : EntityParent
{
    public void Init(Transform character)
    {
        character.tag = "Player";
        animator = character.GetComponent<Animator>();
        audioSource = character.GetComponent<AudioSource>();
        if (null == audioSource)
        {
            audioSource = character.gameObject.AddComponent<AudioSource>();
        }
        GameObject = character.gameObject;
        Transform = character;
        entityActor = GameObject.GetComponent<EntityActor>();
        if (entityActor == null)
        {
            entityActor = GameObject.AddComponent<EntityActor>();
        }
        entityActor.entity = this;
        GatherRenderNodeInfo(GameObject);
    }
    public override void GatherRenderNodeInfo(GameObject rootRenderNode)
    {
        bindNodeTable[CommonDefineBase.BindCenter] = rootRenderNode.transform.Find(CommonDefineBase.BindCenter);
        bindNodeTable[CommonDefineBase.BindRightHand] = rootRenderNode.transform.Find(CommonDefineBase.BindRightHand);
        bindNodeTable[CommonDefineBase.BindQiangkou] = rootRenderNode.transform.Find(CommonDefineBase.BindQiangkou);
        bindNodeTable[CommonDefineBase.BindRightFoot] = rootRenderNode.transform.Find(CommonDefineBase.BindRightFoot);
    }
}
