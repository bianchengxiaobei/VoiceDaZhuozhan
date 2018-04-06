using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class LineRangeSkillActor : SkillActor
{
    private LineRangeSkill gameSkill;
    private AudioSource audio;
    private bool bCanUpdate = false;
    private float liftTime = 0;
    private float time = 0;
    private float intervalTime = 2;
    private DemageType damegeType;
    private RangeType rangeType;
    private ShapeType shapeType;
    private Vector2 param1;
    private Vector2 param2;
    private List<Collider2D> hitList = new List<Collider2D>();
    public Vector2 MoniEntityPos = Vector2.zero;
    public override void CreateEntity()
    {
        gameSkill = (LineRangeSkill)skill;
        string effectPath = this.skillConfig.effectPath;
        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.LogError("effectPath == null :" + this.skillConfig.skillId);
            return;
        }
        switch (this.skillConfig.param[0].name)
        {
            case "伤害":
                this.damegeType = DemageType.伤害;
                break;
            case "减速":
                this.damegeType = DemageType.减速;
                break;
            case "控制":
                this.damegeType = DemageType.控制;
                break;
        }
        switch (this.skillConfig.param[2].name)
        {
            case "持续":
                this.rangeType = RangeType.持续;
                this.liftTime = this.skillConfig.param[2].param;
                break;
            case "瞬发":
                this.rangeType = RangeType.瞬发;
                this.liftTime = this.skillConfig.param[2].param;
                break;
        }
        switch (this.skillConfig.param[3].name)
        {
            case "区域":
                this.shapeType = ShapeType.区域;
                break;
            case "圆形":
                this.shapeType = ShapeType.圆形;
                break;
            case "Box":
                this.shapeType = ShapeType.Box;
                break;
        }
        if (this.shapeType == ShapeType.区域)
        {
            param1 = new Vector2(this.skillConfig.param[4].param, this.skillConfig.param[5].param);//A点
            param2 = new Vector2(this.skillConfig.param[6].param, this.skillConfig.param[7].param);//B点
        }
        else if (this.shapeType == ShapeType.Box)
        {
            param1 = new Vector2(this.skillConfig.param[4].param, this.skillConfig.param[5].param);//pos
            param2 = new Vector2(this.skillConfig.param[6].param, this.skillConfig.param[7].param);//size
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
                    this.theEntity.gameObject.SetActive(true);
                    this.MoniEntityPos = this.theEntity.position;
                    //音频
                    if (this.audio)
                        this.audio.Play();
                });
            }
            else
            {
                this.bCanUpdate = true;
                this.theEntity.gameObject.SetActive(true);
                this.MoniEntityPos = this.theEntity.position;
                //音频
                if (this.audio)
                    this.audio.Play();
            }
        });
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
        this.intervalTime = 2;
        this.hitList.Clear();
        if (this.theEntity != null)
            GameObject.Destroy(this.theEntity.gameObject);
    }
    public override void OnUpdate()
    {
        if (rangeType == RangeType.持续)
        {
            if (this.theEntity != null && bCanUpdate && gameSkill.Test == false)
            {
                intervalTime += Time.deltaTime;
                time += Time.deltaTime;
                if (intervalTime > 2 && time <= liftTime)
                {
                    Collider2D[] hit = Physics2D.OverlapAreaAll((Vector2)this.theEntity.position + param1, (Vector2)this.theEntity.position + param2, 1 << LayerMask.NameToLayer("Enemy"));
                    if (hit != null && hit.Length > 0)
                    {
                        switch (this.damegeType)
                        {
                            case DemageType.伤害:
                                if (gameSkill.Test == false)
                                {
                                    UnityTool.CameraShake.Shake();
                                }
                                foreach (var h in hit)
                                {
                                    this.gameSkill.Attack(h.GetComponent<MonsterActor>().entity);
                                }
                                break;
                        }
                    }
                    intervalTime = 0;
                }
                if (time > liftTime)
                {
                    this.skill.Exit();
                    bCanUpdate = false;
                }
            }
            else if (this.theEntity != null && bCanUpdate && gameSkill.Test == true)
            {
                time += Time.deltaTime;
                if (time > liftTime)
                {
                    this.skill.Exit();
                    bCanUpdate = false;
                }
            }
        }
        else if (rangeType == RangeType.瞬发)
        {
            if (this.theEntity != null && bCanUpdate && gameSkill.Test == false)
            {
                time += Time.deltaTime;
                if (time > liftTime)
                {
                    this.skill.Exit();
                    bCanUpdate = false;
                }
                intervalTime += Time.deltaTime;
                if (intervalTime > 0.5f)
                {
                    intervalTime = 0;
                    Collider2D hit = Physics2D.OverlapBox(MoniEntityPos + param1, param2, 0, 1 << LayerMask.NameToLayer("Enemy"));
                    MoniEntityPos += new Vector2(Time.deltaTime * 6, 0);
                    if (hit != null)
                    {
                        if (this.hitList.Contains(hit))
                        {
                            return;
                        }
                        switch (this.damegeType)
                        {
                            case DemageType.伤害:
                                if (gameSkill.Test == false)
                                {
                                    UnityTool.CameraShake.Shake();
                                }
                                this.gameSkill.Attack(hit.GetComponent<MonsterActor>().entity);
                                break;
                        }
                    }
                }
            }
            else if (this.theEntity != null && bCanUpdate && gameSkill.Test == true)
            {
                time += Time.deltaTime;
                if (time > liftTime)
                {
                    this.skill.Exit();
                    bCanUpdate = false;
                }
            }
        }        
    }

    //if (liftTime > 0)
    //{
    //    time += Time.deltaTime;
    //    if (time >= liftTime)
    //    {
    //        this.skill.Exit();
    //        bCanUpdate = false;
    //    }
    //}
}
public enum DemageType
{
    伤害,
    减速,
    控制
}
public enum RangeType
{
    持续,
    瞬发
}
public enum ShapeType
{
    区域,
    圆形,
    Box
}
