using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CaomaoFramework;
public class FixAreaSkillActor : SkillActor
{
    private Rigidbody2D rigidBody2D;
    private FixAreaSkill gameSkill;
    private bool bCanUpdate = false;
    private AudioSource audio;
    public static Vector2 TestForce = Vector2.one;
    public static Vector2 NormalForce = Vector2.one * 1.2f;
    public override void CreateEntity()
    {
        gameSkill = (FixAreaSkill)skill;
        string effectPath = this.skillConfig.effectPath;
        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.LogError("effectPath == null :" + this.skillConfig.skillId);
            return;
        }
        WWWResourceManager.Instance.Load(effectPath, (asset) => 
        {
            if (asset == null)
            {
                Debug.LogError("asset == null");
                return;
            }
            Vector2 force = gameSkill.Test ? TestForce : NormalForce;
            this.theEntity = asset.Instantiate().transform;
            this.theEntity.gameObject.SetActive(false);
            this.SetSpawPos();
            if (theOwner is EntityLearn)
            {
                this.theEntity.gameObject.layer = LayerMask.NameToLayer("UI");
                this.theEntity.GetComponent<SkillUpdate>().test = true;
            }
            this.audio = this.theEntity.GetComponent<AudioSource>();
            if (this.gameSkill.EffectDelay > 0)
            {
                TimerManager.AddTimer(this.gameSkill.EffectDelay, 0, () =>
                {
                    this.bCanUpdate = true;
                    this.theEntity.gameObject.SetActive(true);
                    this.rigidBody2D = this.theEntity.GetComponent<Rigidbody2D>();
                    if (this.rigidBody2D != null)
                        this.rigidBody2D.AddForce(force * theOwner.entityActor.forceValue);
                    //音频
                    if (this.audio)
                        this.audio.Play();
                });
            }
            else
            {
                this.bCanUpdate = true;
                this.theEntity.gameObject.SetActive(true);
                this.rigidBody2D = this.theEntity.GetComponent<Rigidbody2D>();
                if (this.rigidBody2D != null)
                    this.rigidBody2D.AddForce(force * theOwner.entityActor.forceValue);
                //音频
                if (this.audio)
                    this.audio.Play();
            }
        });
    }
    public override void OnUpdate()
    {
        if (this.theEntity != null && bCanUpdate)
        {
            Collider2D hit = Physics2D.OverlapCircle(this.theEntity.position, 0.9f,1<<LayerMask.NameToLayer("Floor"));
            if (hit != null)
            {
                if (gameSkill.Test == false)
                {
                    UnityTool.CameraShake.Shake();
                }
                this.gameSkill.CaculateDamage();
                this.skill.Exit();               
            }
        }
    }
    public override void Exit()
    {
        if (this.m_bHasExited)
        {
            return;
        }
        base.Exit();
        this.bCanUpdate = false;
        if (this.theEntity != null)
            GameObject.Destroy(this.theEntity.gameObject);
    }
}
