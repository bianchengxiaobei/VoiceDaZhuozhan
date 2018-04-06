using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using System;
public class ResourcePoolManager : Singleton<ResourcePoolManager>
{
    public Dictionary<string, Transform> dicEffectPool = new Dictionary<string, Transform>();
    public Dictionary<AudioType, AudioClip> dicAudioPool = new Dictionary<AudioType, AudioClip>();
    private SpawnPool pool;
    public void Init()
    {
        this.pool = PoolManager.Pools.Create(PoolManager.EffectPoolName);
        this.pool.group.parent = UnityMonoDriver.s_instance.transform.parent;
    }
    public void PreLoad()
    {
        foreach (var data in DataEntityList.dataMap)
        {
            WWWResourceManager.Instance.Load(data.Value.ModelFile, (assetRequest) =>
            {
                Transform assetTra = (assetRequest.mainObject as GameObject).transform;
                GameObject gameobject = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
                PoolManager.Pools[PoolManager.EffectPoolName].Despawn(gameobject.transform);
                ResourcePoolManager.singleton.dicEffectPool.Add(data.Value.ModelFile, assetTra);
            });
        }
        foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
        {
            this.LoadAudio(type);
        }
        WWWResourceManager.Instance.Load(CommonDefineBase.SlowEffectPath, (asset) =>
        {
            Transform assetTra = (asset.mainObject as GameObject).transform;
            GameObject obj = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(obj.transform);
            ResourcePoolManager.singleton.dicEffectPool.Add(CommonDefineBase.SlowEffectPath, assetTra);
        });
        this.PreloadLineAndTarget();
    }
    private void LoadAudio(AudioType type)
    {
        dicAudioPool.Add(type, null);
        EnumAttribute attr = (EnumAttribute)(type.GetType().GetField(type.ToString()).GetCustomAttributes(false)[0]);
        WWWResourceManager.Instance.Load(attr.Description, (asset) =>
        {
            if (asset != null && asset.mainObject != null)
            {
                dicAudioPool[type] = asset.mainObject as AudioClip;
                asset.Retain();
            }
            else
            {
                Debug.LogError("Audio Error:" + type);
            }
        });
    }
    public AudioClip GetAudioClip(AudioType type)
    {
        AudioClip clip;
        if (this.dicAudioPool.TryGetValue(type, out clip))
        {
            return clip;
        }
        Debug.LogError("clip == null");
        return null;
    }
    public void PreloadLineAndTarget()
    {
        WWWResourceManager.Instance.Load(CommonDefineBase.TargetEffectPath, (asset) =>
        {
            Transform assetTra = (asset.mainObject as GameObject).transform;
            GameObject obj = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
            GameObject obj2 = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(obj.transform);
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(obj2.transform);
            ResourcePoolManager.singleton.dicEffectPool.Add(CommonDefineBase.TargetEffectPath, assetTra);
        });
        WWWResourceManager.Instance.Load(CommonDefineBase.LineEffectPath, (asset) =>
        {
            Transform assetTra = (asset.mainObject as GameObject).transform;
            GameObject obj = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
            GameObject obj2 = PoolManager.Pools[PoolManager.EffectPoolName].Spawn(assetTra).gameObject;
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(obj.transform);
            PoolManager.Pools[PoolManager.EffectPoolName].Despawn(obj2.transform);
            ResourcePoolManager.singleton.dicEffectPool.Add(CommonDefineBase.LineEffectPath, assetTra);
        });
    }
}
public enum AudioType
{
    [EnumAttribute("Assets.Audios.Effects.WenShenHit.wav")]
    纹身男死亡,
    [EnumAttribute("Assets.Audios.Effects.PangZiHit.wav")]
    胖子男死亡,
    [EnumAttribute("Assets.Audios.SkillEffects.全屏斩.wav")]
    龙之斩,
    [EnumAttribute("Assets.Audios.SkillEffects.老鹰.wav")]
    飞鹰鸡西,
    [EnumAttribute("Assets.Audios.SkillEffects.诸葛弹琴.wav")]
    召唤诸葛,
    [EnumAttribute("Assets.Audios.SkillEffects.鼻毛音效.wav")]
    召唤圣高
}
