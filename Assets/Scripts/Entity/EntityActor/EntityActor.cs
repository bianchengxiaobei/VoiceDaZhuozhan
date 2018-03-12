using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：EntityActor
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>
public class EntityActor : MonoBehaviour
{
    public EntityParent entity;
    public Vector3 moveDir = Vector3.zero;
    private void Awake()
    {

    }
    public virtual void Move()
    {

    }
    public virtual void Idle()
    {

    }
}
