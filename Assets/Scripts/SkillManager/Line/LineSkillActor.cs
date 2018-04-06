using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：LineSkillActor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class LineSkillActor : SkillActor
{
    Vector3 moveDir = Vector3.right;
    public override void CreateEntity()
    {
        if (theOwner is EntityMonster)
        {
            moveDir = Vector3.left;
        }
        if (string.IsNullOrEmpty(this.skillConfig.effectPath))
        {
            return;
        }
        switch (this.skillConfig.effectPos)
        {
            case EffectBindPos.中心:
                spawPos = theOwner.GetBindNodePos("Center");
                break;
            case EffectBindPos.右手:
                spawPos = theOwner.GetBindNodePos("RightHandPos");
                break;
            case EffectBindPos.枪口:
                spawPos = theOwner.GetBindNodePos("Qiangkou");
                break;

        }
        WWWResourceManager.Instance.Load(skillConfig.effectPath, (asset) =>
        {
            if (asset != null)
            {
                GameObject effect = asset.Instantiate();
                effect.transform.SetParent(theOwner.Transform);
                effect.transform.localPosition = spawPos;
                effect.transform.localScale = Vector3.one;
            }
        });
    }
    public override void OnUpdate()
    {
        
    }
}
