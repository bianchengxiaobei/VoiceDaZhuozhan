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
        entity.Transform.Translate(moveDir * Time.deltaTime * 1.5f);
    }
}
