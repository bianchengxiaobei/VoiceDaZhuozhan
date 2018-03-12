using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
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
    public static Vector3 TiaotiaoBornPos = new Vector3(31, 0, 0);
    private uint AllDuration = 0;
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
                    if (!this.curWaveDicEntitys.ContainsKey(i))
                    {
                        List<int> entityIndex = new List<int>();
                        int index = ++WaveEntityIndex;
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
                //int num = 0;
                //if (master.num > 6)
                //{
                //    num = 6;
                //}
                //else
                //{
                //    num = master.num;
                //}
                int num = master.num;
                for (int i = 0; i < num; i++)
                {
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
    public void LoadAllEntity()
    {
        foreach (var entity in this.dicCreateEntitys.Values)
        {
            entity.CreateEntity(()=> 
            {
                //注册说话回调

            });
        }
    }
    public void ShowEntity(EntityParent entity,Vector3 pos,float duration = 0)
    {
        entity.Transform.position = pos;
        if (entity != null)
        {
            if (duration != 0)
            {
                this.AllDuration += (uint)(duration * 1000);
                TimerManager.AddTimer(this.AllDuration, 0, () => { entity.SetVisiable(true); entity.entityActor.Move(); allVisiableMonster.Add(entity); });
            }
            else
            {
                entity.SetVisiable(true);
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
                return PlayerManager.wenShenBornPos;
            case EntityType.跳跳男:
                break;
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
    }
}
