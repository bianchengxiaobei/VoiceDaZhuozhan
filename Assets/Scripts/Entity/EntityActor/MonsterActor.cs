using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：EntityMonster
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class MonsterActor : EntityActor
{
    public override void Move()
    {
        this.moveDir = Vector3.left;
        this.entity.Move();
    }
    public override void Idle()
    {
        this.moveDir = Vector3.zero;
        this.entity.Idle();
    }
    private void Update()
    {
        if (entity.CurrentMotionState.Equals(MotionState.WALKING))
        {
            entity.Transform.Translate(moveDir * Time.deltaTime * this.entity.speedRate);
        }
        if (this.entity != null && this.entity.fsmMotion != null)
        {
            this.entity.fsmMotion.Execute(this.entity);
        }
        if (PlayerManager.singleton.MySelf == null || PlayerManager.singleton.MySelf.bIsDead)
        {
            return;
        }
        if (Vector2.Distance(entity.Transform.position, PlayerManager.singleton.MySelf.Transform.position) <= 4)
        {
            entity.Attack();
        }
    }
}
