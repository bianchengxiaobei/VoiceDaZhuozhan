using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class LockSkillActor : SkillActor
{
    private LockSkill gameSkill;
    private AudioSource audio;
    private bool bCanUpdate = false;
    private int liftTime = 0;
    private float time = 0;
    public override void CreateEntity()
    {
        gameSkill = (LockSkill)skill;
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
            this.theEntity = asset.Instantiate().transform;
            this.theEntity.gameObject.SetActive(false);
            this.SetSpawPos();
            if (theOwner is EntityLearn)
            {
                this.theEntity.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            this.audio = this.theEntity.GetComponent<AudioSource>();
            if (this.gameSkill.EffectDelay > 0)
            {
                TimerManager.AddTimer(this.gameSkill.EffectDelay, 0, () =>
                {
                    this.bCanUpdate = true;
                    this.liftTime = 12;
                    this.theEntity.gameObject.SetActive(true);
                    //音频
                    if (this.audio)
                        this.audio.Play();
                });
            }
            else
            {
                this.bCanUpdate = true;
                this.liftTime = 12;
                this.theEntity.gameObject.SetActive(true);
                //音频
                if (this.audio)
                    this.audio.Play();
            }
        });
    }
    public override void OnUpdate()
    {
        if (this.theEntity != null && bCanUpdate && gameSkill.Test == false)
        {
            this.theEntity.transform.Translate(Vector2.right * Time.deltaTime * 6);
            Collider2D hit = Physics2D.OverlapCircle(this.theEntity.position, 2f, 1 << LayerMask.NameToLayer("Enemy"));
            if (hit != null)
            {
                //if (!hit.GetComponent<MonsterActor>().entity.bIsDead)
                //{
                if (gameSkill.Test == false)
                {
                    UnityTool.CameraShake.Shake();
                }
                this.gameSkill.CaculateDamage();
                this.skill.Exit();
                //}
            }
        }
        else if(this.theEntity != null && bCanUpdate && gameSkill.Test == true)
        {
            this.theEntity.transform.Translate(Vector2.right * Time.deltaTime * 6);
            Collider2D hit = Physics2D.OverlapCircle(this.theEntity.position, 2f, 1 << LayerMask.NameToLayer("Mu"));
            if (hit != null)
            {
                this.skill.Exit();
            }
        }
        if (liftTime > 0)
        {
            time += Time.deltaTime;
            if (time >= liftTime)
            {
                this.skill.Exit();
                bCanUpdate = false;
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
        this.time = 0;
        if (this.theEntity != null)
            GameObject.Destroy(this.theEntity.gameObject);
    }
}
