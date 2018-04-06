using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
public class SkillUpdate : MonoBehaviour
{
    public GameObject effectObj;
    public float time;
    public AudioClip clip;
    private GameObject ins;
    public bool test = false;
    private void OnDestroy()
    {
        if (effectObj != null)
        {
            ins = GameObject.Instantiate(effectObj);
            if (test)
                ins.layer = LayerMask.NameToLayer("UI");
            ins.transform.position = this.transform.position;
            AudioManagerBase.Instance.PlayUIEffectSound(clip);
            Destroy(ins, time);
            //Destroy(clip, time);
        }
    }
}
