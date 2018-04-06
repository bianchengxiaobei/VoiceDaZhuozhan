using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class SelectDirSkillActor : SkillActor
{
    private SelectDirSkill gameSkill;
    private bool bCanUpdate;
    private AudioSource audio;
    private TargetBehaviour TargetBehaviour;
    private Line line;
    private Vector2 dir;
    private int liftTime = 0;
    private float time = 0;
    public override void Enter(Skill skill, EntityParent theOwner, GameSkillBase gameskill)
    {
        base.Enter(skill, theOwner, gameskill);
        //实例化箭头
        if (ResourcePoolManager.singleton.dicEffectPool.ContainsKey(CommonDefineBase.TargetEffectPath))
        {
            Transform prefab = ResourcePoolManager.singleton.dicEffectPool[CommonDefineBase.TargetEffectPath];
            GameObject gameobject = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(prefab).gameObject;
            if (gameSkill.Test)
            {
                gameobject.transform.position = Vector3.zero;
                gameobject.layer = LayerMask.NameToLayer("UI");
            }
            else
            {
                gameobject.transform.position = new Vector3(12, 8, 0);
                gameobject.layer = LayerMask.NameToLayer("Default");
            }
            TargetBehaviour = gameobject.GetComponent<TargetBehaviour>();
            TargetBehaviour.text.gameObject.SetActive(true);
            TargetBehaviour.skill = gameskill as SelectDirSkill;
        }
        if (ResourcePoolManager.singleton.dicEffectPool.ContainsKey(CommonDefineBase.LineEffectPath))
        {
            Transform prefab = ResourcePoolManager.singleton.dicEffectPool[CommonDefineBase.LineEffectPath];
            GameObject gameobject = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(prefab).gameObject;
            if (gameSkill.Test)
            {
                gameobject.layer = LayerMask.NameToLayer("UI");
                Vector3 pos = gameobject.transform.position;
                pos.z = 24;
                gameobject.transform.position = pos;
            }
            else
            {
                gameobject.layer = LayerMask.NameToLayer("Default");
            }
            line = gameobject.GetComponent<Line>();
            line.Initialise((theOwner.bindNodeTable[CommonDefineBase.BindQiangkou] as Transform).position,TargetBehaviour.transform.position, 0.05f, Color.red); ;
        }
    }
    public void OpenFire()
    {
        dir = (line.end - line.start).normalized;
        theOwner.PlayAnimation(gameSkill.thirdAnim);
        WWWResourceManager.Instance.Load(this.skillConfig.effectPath, (asset) =>
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
                    this.liftTime = 4;
                    this.theEntity.gameObject.SetActive(true);
                    //音频
                    if (this.audio)
                        this.audio.Play();
                    if (gameSkill.Test == false)
                    {
                        UnityTool.CameraShake.Shake();
                    }
                });
            }
            else
            {
                this.bCanUpdate = true;
                this.liftTime = 4;
                this.theEntity.gameObject.SetActive(true);
                //音频
                if (this.audio)
                    this.audio.Play();
                if (gameSkill.Test == false)
                {
                    UnityTool.CameraShake.Shake();
                }
            }
        });
    }
    public void EndFire()
    {
        TimerManager.AddTimer(this.gameSkill.EffectDelay, 0, () =>
        {
            if (PlayerManager.singleton.MySelf != null)
            {
                PlayerManager.singleton.MySelf.PlayAnimation("Idle");
            }
            else
            {
                theOwner.PlayAnimation("Idle");
            }
            this.TargetBehaviour.Clear();
            if (this.TargetBehaviour.gameObject.activeSelf == true)
            {
                PoolManager.Pools[PoolManager.EffectPoolName].Despawn(this.TargetBehaviour.transform);
                PoolManager.Pools[PoolManager.EffectPoolName].Despawn(this.line.transform);
            }
        });
    }
    public override void CreateEntity()
    {
        gameSkill = (SelectDirSkill)skill;
        string effectPath = this.skillConfig.effectPath;
        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.LogError("effectPath == null :" + this.skillConfig.skillId);
            return;
        }
    }
    public override void OnUpdate()
    {
        if (line != null && TargetBehaviour != null)
        {
            line.end = TargetBehaviour.transform.position;
        }
        if (theEntity != null && bCanUpdate && gameSkill.Test == false)
        {
            theEntity.transform.Translate(dir * Time.deltaTime * 20);
            Collider2D hit = Physics2D.OverlapCircle(this.theEntity.position, 0.6f, 1 << LayerMask.NameToLayer("Enemy"));
            if (hit != null)
            {
                this.gameSkill.CaculateDamage(hit.GetComponent<MonsterActor>().entity);
                this.skill.Exit();
                return;
            }
            Collider2D hit2 = Physics2D.OverlapCircle(this.theEntity.position, 0.6f, 1 << LayerMask.NameToLayer("Floor"));
            if (hit2 != null)
            {
                this.skill.Exit();
                return;
            }
        }
        else if (theEntity != null && bCanUpdate && gameSkill.Test == true)
        {
            theEntity.transform.Translate(dir * Time.deltaTime * 20);
            Collider2D hit = Physics2D.OverlapCircle(this.theEntity.position, 0.6f, 1 << LayerMask.NameToLayer("Mu"));
            if (hit != null)
            {
                this.skill.Exit();
                return;
            }
            Collider2D hit2 = Physics2D.OverlapCircle(this.theEntity.position, 0.6f, 1 << LayerMask.NameToLayer("Floor"));
            if (hit2 != null)
            {
                this.skill.Exit();
                return;
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
        if (this.TargetBehaviour.gameObject.activeSelf == true)
        {
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(this.TargetBehaviour.transform);
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(this.line.transform);
        }
        this.line = null;
        this.TargetBehaviour = null;
        if (this.theEntity != null)
            GameObject.Destroy(this.theEntity.gameObject);
    }
}
