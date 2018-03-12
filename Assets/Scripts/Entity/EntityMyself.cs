using System;
using UnityEngine;
public class EntityMyself : EntityParent
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
            entityActor = GameObject.GetComponent<EntityActor>();
            if (entityActor != null)
                entityActor.entity = this;
            Renderer = gameobject.GetComponentInChildren<Renderer>();
            Transform.gameObject.layer = 8;
            GatherRenderNodeInfo(GameObject);
            this.SetVisiable(false);
            if (action != null)
            {
                action();
            }
        });
    }
    public void CastSkill(string skillToken)
    {
        GameSkillBase skill = SkillManager.singleton.GetSkill(skillToken);
        if (skill != null)
        {
            this.ChangeMotionState(MotionState.ReleaseSkill,skill);
        }
    }
}

