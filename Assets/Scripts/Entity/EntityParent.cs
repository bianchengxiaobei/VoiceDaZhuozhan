using System;
using System.Collections;
using UnityEngine;
using CaomaoFramework;
public class EntityParent
{
    public int EntityId;
    public bool bIsDead;
    public string name;
    public int type;
    public int maxHp;
    public int curHp;
    public float speed;
    public int demage;
    public Animator animator;
    public FSMParent fsmMotion;
    public EntityActor entityActor;
    public AudioSource audioSource;
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
    public virtual void SetEntityInfo(EntityInfo info)
    {
        if (info != null)
        {
            this.EntityId = info.id;
            this.speed = info.speed;
            this.type = info.EntityType;
            this.maxHp = info.maxHp;
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
    public void GatherRenderNodeInfo(GameObject rootRenderNode)
    {
        bindNodeTable["Center"] = rootRenderNode.transform.Find("Center");
        bindNodeTable["RightHandPos"] = rootRenderNode.transform.Find("RightHandPos");
        bindNodeTable["Qiangkou"] = rootRenderNode.transform.Find("Qiangkou");
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
        if (this.currentMotionState != MotionState.WALKING)
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
    public void ApplyDemage(float deamge)
    {
        this.curHp -= (int)deamge;
        if (this.curHp <= 0)
        {
            this.Dead();
        }
    }
    public void ApplySlowDown(int downValue)
    {
        float value = this.speed;
        switch (downValue)
        {
            case 1:
                value = this.speed * 0.3f;
                break;
            case 2:
                value = this.speed * 0.5f;
                break;
            case 3:
                value = this.speed * 0.7f;
                break;
        }
        this.SetMoveSpeed(value);
    }
    public void ApplyControl(float timeValue)
    {
        this.ChangeMotionState(MotionState.Controll, timeValue);
    }
    public void OnHit()
    {
        if (this.currentMotionState != MotionState.HIT)
        {
            this.ChangeMotionState(MotionState.HIT);
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
        this.speed = speedValue;
        //更改动画速度
    }
    public Vector3 GetBindNodePos(string bind)
    {
        if (bindNodeTable.ContainsKey(bind))
        {
            return (bindNodeTable[bind] as Transform).localPosition;
        }
        return Vector3.zero;
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
}