using System;
using System.Collections;
using UnityEngine;
using CaomaoFramework;
using System.Collections.Generic;
public class EntityParent
{
    public int EntityId;
    public bool bIsDead;
    public string name;
    public int type;
    public int maxHp;
    public int curHp;
    public float speed;
    public float maxSpeed;
    public int demage;
    public Animator animator;
    public FSMParent fsmMotion;
    public EntityActor entityActor;
    public AudioSource audioSource;
    public Collider2D collider2d;
    public float speedRate;
    public List<uint> timer = new List<uint>();
    public List<GameObject> Effcets = new List<GameObject>();
    protected string currentMotionState = MotionState.IDLE;
    /// <summary>
    /// 现在的Entity状态，默认为Idle
    /// </summary>
    public string CurrentMotionState
    {
        get
        {
            return this.currentMotionState;
        }
        set
        {
            currentMotionState = value;
        }
    }
    public GameObject GameObject { get; set; }
    public Transform Transform { get; set; }
    public Renderer Renderer { get; set; }
    public bool IsVisiable = false;//角色是否可见
    public Vector3 Position = Vector3.zero;
    public Vector3 Rotation = Vector3.zero;
    //挂载点数据
    public Hashtable bindNodeTable = new Hashtable();
    public virtual void EnterLevel()
    {

    }
    public virtual void CreateEntity(Action action)
    {

    }
    public virtual void LeaveLevel()
    {
        collider2d.enabled = false;
        if (this is EntityMyself)
        {
            //游戏结束
            TimerManager.AddTimer(2000, 0, () =>
            {
                PlayerManager.singleton.OverGame();
                this.Clear();
            });
            return;
        }
        PlayerManager.singleton.allVisiableMonster.Remove(this);
        TimerManager.AddTimer(4000, 0, () =>
        {
            PlayerManager.singleton.curWaveDicEntitys[LevelManager.singleton.curWaveIndex].Remove(EntityId);
            if (!PlayerManager.singleton.curWaveDicEntitys.ContainsKey(LevelManager.singleton.curWaveIndex))
            {
                Debug.LogError("Error:" + LevelManager.singleton.curWaveIndex);
            }
            if (PlayerManager.singleton.curWaveDicEntitys[LevelManager.singleton.curWaveIndex].Count == 0)
            {
                PlayerManager.singleton.EndWave();
            }
            this.Clear();
        });
    }
    public void Clear()
    {
        if (this is EntityMyself && PlayerManager.singleton.MySelf == null)
        {
            return;
        }
        if (this.timer.Count != 0)
        {
            foreach (var id in timer)
            {
                TimerManager.DelTimer(id);
            }
            this.timer.Clear();
        }
        if (this.Effcets.Count != 0)
        {
            foreach (var effect in this.Effcets)
            {
                PoolManager.Pools[PoolManager.EffectPoolName].Despawn(effect.transform);
            }
            this.Effcets.Clear();
        }
        PoolManager.Pools[PoolManager.EffectPoolName].Despawn(Transform);
        this.collider2d.enabled = false;
        this.entityActor.enabled = false;
        this.animator = null;
        this.fsmMotion = null;
        _2dxFX_DesintegrationFX deadFx = this.GameObject.GetComponent<_2dxFX_DesintegrationFX>();
        if (deadFx != null)
        {
            deadFx.Desintegration = 0;
        }
        this.entityActor = null;
        this.audioSource.clip = null;
        this.audioSource = null;
    }
    public virtual void SetEntityInfo(EntityInfo info)
    {
        if (info != null)
        {
            this.EntityId = info.id;
            this.speed = info.speed;
            this.maxSpeed = info.speed;
            this.speedRate = this.speed / this.maxSpeed;
            this.type = info.EntityType;
            this.maxHp = info.maxHp;
            this.curHp = this.maxHp;
            this.name = info.name;
            this.demage = info.demage;
        }
    }
    public void PlayAnimation(string anim, float duration = 0.0f)
    {
        if (this.animator != null)
        {
            this.animator.CrossFade(anim, duration);
        }
    }
    /// <summary>
    /// 收集挂载点信息
    /// </summary>
    /// <param name="rootRenderNode"></param>
    /// <returns></returns>
    public virtual void GatherRenderNodeInfo(GameObject rootRenderNode)
    {
        bindNodeTable[CommonDefineBase.BindCenter] = rootRenderNode.transform.Find(CommonDefineBase.BindCenter);
        bindNodeTable[CommonDefineBase.BindRightHand] = rootRenderNode.transform.Find(CommonDefineBase.BindRightHand);
        bindNodeTable[CommonDefineBase.BindQiangkou] = rootRenderNode.transform.Find(CommonDefineBase.BindQiangkou);
        bindNodeTable[CommonDefineBase.BindRightFoot] = rootRenderNode.transform.Find(CommonDefineBase.BindRightFoot);
    }
    public void SetVisiable(bool bVisiable)
    {
        this.IsVisiable = bVisiable;
        if (GameObject != null)
        {
            GameObject.SetActive(bVisiable);
        }
    }
    public void Move()
    {
        if (this.currentMotionState != MotionState.WALKING && this.currentMotionState != MotionState.DEAD)
        {
            this.ChangeMotionState(MotionState.WALKING);
        }
    }
    public void Idle()
    {
        if (this.currentMotionState != MotionState.IDLE)
        {
            this.ChangeMotionState(MotionState.IDLE);
        }
    }
    public void Dead()
    {
        if (this.currentMotionState != MotionState.DEAD)
        {
            this.ChangeMotionState(MotionState.DEAD);
        }
    }
    public void ApplyDemage(float deamge,Skill skill)
    {
        this.curHp -= (int)deamge;
        if (this.curHp <= 0)
        {
            this.Dead();
        }
        else
        {
            this.OnHit(skill);
        }
    }
    public void ApplySlowDown(int downValue,float duration)
    {
        float value = this.speed;
        switch (downValue)
        {
            case 1:
                value = this.speed * 0.6f;
                break;
            case 2:
                value = this.speed * 0.4f;
                break;
            case 3:
                value = this.speed * 0.2f;
                break;
        }
        this.SetMoveSpeed(value);
        GameObject slowEffect = null;
        if (ResourcePoolManager.singleton.dicEffectPool.ContainsKey(CommonDefineBase.SlowEffectPath))
        {
            Transform prefab = ResourcePoolManager.singleton.dicEffectPool[CommonDefineBase.SlowEffectPath];
            GameObject obj = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(prefab).gameObject;
            slowEffect = obj;
            this.Effcets.Add(obj);
            obj.transform.SetParent(bindNodeTable[CommonDefineBase.BindRightFoot] as Transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * 2;
        }
        else
        {
            WWWResourceManager.Instance.Load(CommonDefineBase.SlowEffectPath, (asset) =>
            {
                Transform assetTra = (asset.mainObject as GameObject).transform;
                GameObject obj = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
                slowEffect = obj;
                ResourcePoolManager.singleton.dicEffectPool.Add(CommonDefineBase.SlowEffectPath, assetTra);
                this.Effcets.Add(obj);
                obj.transform.SetParent(bindNodeTable[CommonDefineBase.BindRightFoot] as Transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one * 2;
            });
        }
        uint dur = (uint)(duration * 1000);
        this.timer.Add(TimerManager.AddTimer(dur, 0, () => 
        {
            this.SetMoveSpeed(this.maxSpeed);
            if (slowEffect != null)
            {
                PoolManager.Pools[PoolManager.EffectPoolName].Despawn(slowEffect.transform);
                this.Effcets.Remove(slowEffect);
            }
        }));
    }
    public void ApplyControl(float timeValue)
    {
        this.ChangeMotionState(MotionState.Controll, timeValue);
    }
    public void OnHit(Skill skill)
    {
        if (this.currentMotionState != MotionState.HIT)
        {
            this.ChangeMotionState(MotionState.HIT,skill);
        }
    }
    public void Attack()
    {
        if (this.currentMotionState != MotionState.Attack && this.currentMotionState != MotionState.DEAD)
        {
            this.ChangeMotionState(MotionState.Attack);
        }
    }
    /// <summary>
    /// 改变状态机的新状态
    /// </summary>
    /// <param name="newState">新状态</param>
    /// <param name="args"></param>
    public virtual void ChangeMotionState(string newState, params object[] args)
    {
        fsmMotion.ChangeStatus(this, newState, args);
    }
    public void SetSpeed(float speed)
    {
        this.animator.SetFloat("Speed", speed);
    }
    public void SetMoveSpeed(float speedValue)
    {
        //float rate = speedValue / this.speed;
        this.speed = speedValue;
        this.speedRate = this.speed / this.maxSpeed;
        //更改动画速度
        this.animator.speed = this.speedRate;
    }
    public Vector3 GetBindNodePos(string bind)
    {
        if (bindNodeTable.ContainsKey(bind))
        {
            return (bindNodeTable[bind] as Transform).localPosition;
        }
        return Vector3.zero;
    }
    public void SetAction(int action,float duration)
    {
        this.animator.SetInteger("Action", action);
        uint dur = (uint)(duration * 1000);
        TimerManager.AddTimer(dur, 0, () => 
        {
            this.animator.SetInteger("Action", 0);
        });
    }
}
public class EntityInfo
{
    public int EntityType;
    public int id;
    public int maxHp;
    public int demage;
    public float speed;
    public string name;
    public EntityInfo(int id,int type, int maxHp, int demage, string name,float speed)
    {
        this.id = id;
        this.EntityType = type;
        this.maxHp = maxHp;
        this.demage = demage;
        this.name = name;
        this.speed = speed;
    }
}

public static class MotionState
{
    static readonly public string IDLE = "idle";
    static readonly public string WALKING = "walking";
    static readonly public string Controll = "Controll";
    static readonly public string ReleaseSkill = "ReleaseSkill";
    static readonly public string DEAD = "dead";
    static readonly public string HIT = "hit";
    static readonly public string Attack = "attack";
}