using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
/*----------------------------------------------------------------
// 模块名：PlayerManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    public Dictionary<int, EntityParent> dicCreateEntitys = new Dictionary<int, EntityParent>();
    public Dictionary<int, List<int>> curWaveDicEntitys = new Dictionary<int, List<int>>();
    public List<EntityParent> allVisiableMonster = new List<EntityParent>();
    public EntityMyself MySelf;
    public static int CreateEntityIndex = 0;
    public static int WaveEntityIndex = 0;
    public static Vector3 wenShenBornPos = new Vector3(31, 0, 0);
    public static Vector3 FeijiBornPos = new Vector3(31, 15, 0);
    private uint AllDuration = 0;
    private List<uint> timer = new List<uint>();
    public void CreateAllEntity()
    {
        this.CreateMyself();
        Level level = LevelManager.singleton.GetCurLevel();
        int waveSize = level.waves.Count;
        for (int i = 0; i < waveSize; i++)
        {
            for (int j = 0; j < level.waves[i].entitys.Count; j++)
            {
                MasterWave master = level.waves[i].entitys[j];
                for (int k = 0; k < master.num; k++)
                {
                    //不包含当前波数
                    if (!this.curWaveDicEntitys.ContainsKey(i))
                    {
                        List<int> entityIndex = new List<int>();
                        int index = ++WaveEntityIndex;
                        //排除主角索引id
                        if (index == 1)
                        {
                            index = ++WaveEntityIndex;
                        }
                        entityIndex.Add(index);
                        this.curWaveDicEntitys.Add(i, entityIndex);
                    }
                    else
                    {
                        this.curWaveDicEntitys[i].Add(++WaveEntityIndex);
                    }
                }
            }
        }
        this.StartWave(level,LevelManager.singleton.curWaveIndex);
    }
    public void StartWave(Level level,int index)
    {
        if (level != null && level.waves.Count > 0)
        {
            Wave firstWave = level.waves[index];
            foreach (var master in firstWave.entitys)
            {
                int num = master.num;
                for (int i = 0; i < num; i++)
                {
                    //取出当前波数的怪物创建
                    this.CreateMaster(master);
                }
            }
        }
    }
    public void CreateMyself()
    {
        EntityParent myself = new EntityMyself();
        this.MySelf = myself as EntityMyself;
        EntityInfo info = new EntityInfo(++CreateEntityIndex, (int)EntityType.主角,100, 10, EntityType.主角.ToString(), 0);
        myself.SetEntityInfo(info);
        myself.EnterLevel();
        this.dicCreateEntitys.Add(myself.EntityId, myself);
    }
    public void CreateMaster(MasterWave master)
    {
        EntityParent masterEntity = new EntityMonster();
        EntityInfo info = new EntityInfo(++CreateEntityIndex,(int)master.type,master.maxHp,master.demage,master.type.ToString(),master.speed);
        masterEntity.SetEntityInfo(info);
        masterEntity.EnterLevel();
        this.dicCreateEntitys.Add(masterEntity.EntityId, masterEntity);
    }
    public void LoadAllEntity(Action allFinished = null)
    {
        int index = 0;
        foreach (var entity in this.dicCreateEntitys.Values)
        {
            entity.CreateEntity(()=> 
            {
                if (allFinished != null && ++index == this.dicCreateEntitys.Values.Count)
                {
                    allFinished();
                }
            });
        }
    }
    public void UnloadAllEntity()
    {
        foreach (var entity in this.dicCreateEntitys.Values)
        {
            if (entity != null && entity.GameObject != null)
            {
                entity.Clear();
            }
        }
        if (allVisiableMonster.Count > 0)
        {
            this.Clear();
        }
    }
    public void ShowEntity(EntityParent entity,Vector3 pos,float duration = 0)
    {
        entity.Transform.position = pos;
        if (entity != null)
        {
            if (duration > 0)
            {
                this.AllDuration += (uint)(duration * 1000);
                this.timer.Add(TimerManager.AddTimer(this.AllDuration, 0, () =>
                {
                    entity.SetVisiable(true);
                    if (entity.entityActor == null)
                    {
                        Debug.LogError("fwefefef:" + entity.EntityId);
                    }
                    entity.entityActor.Move();
                    allVisiableMonster.Add(entity);
                }));
            }
            else
            {
                entity.SetVisiable(true);
                if (entity is EntityMonster)
                    allVisiableMonster.Add(entity);
                if (entity.entityActor != null)
                     entity.entityActor.Move();
            }
        }
    }
    public void ShowEntity(EntityParent entity)
    {
        float spawTime = LevelManager.singleton.GetRandomSpawTime();
        Debug.Log(spawTime);
        this.ShowEntity(entity, GetEntityBornPos(entity), spawTime);
    }
    public Vector3 GetEntityBornPos(EntityParent entity)
    {
        switch ((EntityType)entity.type)
        {
            case EntityType.纹身男:               
            case EntityType.跳跳男:          
            case EntityType.胖子男:
                return PlayerManager.wenShenBornPos;
            case EntityType.飞机男:
                return PlayerManager.FeijiBornPos;
            default:
                return Vector3.zero;
        }
        return Vector3.zero;
    }
    public void ShowCurWaveMaster()
    {
        List<int> entityIds = this.curWaveDicEntitys[LevelManager.singleton.curWaveIndex];
        if (entityIds.Count > 0)
        {
            List<int> random = UnityTool.RandomList<int>(entityIds);
            for (int i = 0; i < random.Count; i++)
            {
                int entityId = random[i];
                if (this.dicCreateEntitys.ContainsKey(entityId))
                {
                    EntityParent entity = this.dicCreateEntitys[entityId];
                    this.ShowEntity(entity);
                }
                else
                {
                    Debug.Log("没有改Entity：" + entityId);
                }
            }     
        }
    }
    public void ShowMySelf()
    {
        this.ShowEntity(this.MySelf,Vector3.zero);
    }
    public void EndWave()
    {
        this.AllDuration = 0;
        this.dicCreateEntitys.Clear();
        LevelManager.singleton.curWaveIndex += 1;
        if (!this.curWaveDicEntitys.ContainsKey(LevelManager.singleton.curWaveIndex))
        {
            //说明没有下一波,结束当前关卡
            this.Clear();
            LevelManager.singleton.Clear();
            SkillManager.singleton.ClearSkill();
            //通知主界面结束游戏
            EventDispatch.Broadcast<bool>(Events.DlgMainEndLevel, true);
        }
        else
        {
            //开始下一波
            this.StartWave(LevelManager.singleton.GetCurLevel(), LevelManager.singleton.curWaveIndex);
            //刷新界面
            EventDispatch.Broadcast(Events.DlgMainRefresh);
            //加载所有怪物
            PlayerManager.singleton.LoadAllEntity(()=> 
            {
                PlayerManager.singleton.ShowCurWaveMaster();
            });
        }
    }
    public void OverGame()
    {
        this.AllDuration = 0;
        this.dicCreateEntitys.Clear();
        foreach (var entity in allVisiableMonster)
        {
            if (entity != null && entity.GameObject != null)
            {
                entity.Clear();
            }
        }
        this.Clear();
        LevelManager.singleton.Clear();
        SkillManager.singleton.ClearSkill();
        //通知主界面结束游戏
        EventDispatch.Broadcast<bool>(Events.DlgMainEndLevel, false);
    }
    public EntityParent GetEntity(int id)
    {
        if (this.dicCreateEntitys.ContainsKey(id))
        {
            return this.dicCreateEntitys[id];
        }
        return null;
    }
    public List<EntityParent> GetInRangeVisibleEntity(Vector3 position,float rangle)
    {
        List<EntityParent> inRangleList = new List<EntityParent>();
        foreach (var entity in this.allVisiableMonster)
        {
            position.y = entity.Transform.position.y;
            if (Vector2.Distance(position, entity.Transform.position) <= rangle)
            {
                inRangleList.Add(entity);
            }
        }
        return inRangleList;
    }
    public List<EntityParent> GetNearestVisibleEntity(Vector2 self, int num = 1)
    {
        List<EntityParent> nearestList = new List<EntityParent>();
        float min = 1000;
        EntityParent nearEntity = null;
        foreach (var entity in this.allVisiableMonster)
        {
            if (entity.bIsDead)
            {
                continue;
            }
            float dis = Vector2.Distance(self, entity.Transform.position);
            if (dis < min)
            {
                nearEntity = entity;
                min = dis;
            }
        }
        if (nearEntity != null && num == 1)
        {
            nearestList.Add(nearEntity);
        }
        return nearestList;
    }
    public List<EntityParent> GetRandomVisiableEntity(int num)
    {
        List<EntityParent> randowList = new List<EntityParent>();
        int sum = this.allVisiableMonster.Count;
        if (sum == 0)
        {
            return randowList;
        }
        int random = UnityEngine.Random.Range(0, sum);
        randowList.Add(this.allVisiableMonster[random]);
        if (sum == 1)
        {
            goto ed;
        }
        else
        {
            goto agin;
        }
        agin:
        int newRandom = UnityEngine.Random.Range(0, sum);
        if (newRandom == random)
        {
            goto agin;
        }
        else
        {
            randowList.Add(this.allVisiableMonster[newRandom]);
            goto ed;
        }
        ed:
        return randowList;
    }
    public void Clear()
    {
        foreach (var timerId in this.timer)
        {
            TimerManager.DelTimer(timerId);
        }
        this.timer.Clear();
        dicCreateEntitys.Clear();
        curWaveDicEntitys.Clear();
        allVisiableMonster.Clear();
        if (MySelf != null && MySelf.Transform != null)
        {
            MySelf.Clear();
        }
        MySelf = null;
        CreateEntityIndex = 0;
        WaveEntityIndex = 0;
        this.AllDuration = 0;
    }
}
