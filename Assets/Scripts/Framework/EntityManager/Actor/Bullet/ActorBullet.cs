using UnityEngine;
using System.Collections.Generic;
/*----------------------------------------------------------------
// 模块名：ActorBullet
// 创建者：chen
// 修改者列表：
// 创建日期：2017.7.8
// 模块描述：飞行物(包含追踪功能)
//--------------------------------------------------------------*/
/// <summary>
/// 飞行物
/// </summary>
public class ActorBullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public Transform target;
    public Vector3 targetPos;
    //是否初始化完毕
    private bool isSetUp;
    private Rigidbody2D rg;
    public bool isEnemy = false;
    //销毁回调
    public System.Action OnDestroy = null;
    //打到敌人
    public System.Action<int,int> OnHit = null;
    /// <summary>
    /// 设置追踪的目标位置
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="targetPos"></param>
    public void SetUp(Transform target,float speed,float lifeTime,Vector3 targetPos)
    {
        this.target = target;
        this.lifeTime = lifeTime;
        this.targetPos = targetPos;
        if (target != null)
        {
            this.targetPos = target.position;
        }
        this.speed = speed;
        this.isSetUp = true;
    }
    public void Awake()
    {
        this.rg = this.GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        if (!isSetUp)
        {
            return;
        }
        this.OnUpdate();      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnemy && collision.CompareTag("Enemy"))
        {
            if (OnHit != null)
            {
                OnHit(1, collision.gameObject.GetInstanceID());
                OnHit = null;
            }

            if (OnDestroy != null)
            {
                OnDestroy();
                OnDestroy = null;
            }
                
        }
        if (collision.CompareTag("Entity"))
        {
            if (OnHit != null)
            {
                OnHit(2, collision.gameObject.GetInstanceID());
                OnHit = null;
            }
               
            if (OnDestroy != null)
            {
                OnDestroy();
                OnDestroy = null;
            }
        }
        if (collision.CompareTag("Obstruct"))
        {
            if (OnHit != null)
            {
                OnHit(3, collision.gameObject.GetInstanceID());
                OnHit = null;
            }
            if (OnDestroy != null)
            {
                OnDestroy();
                OnDestroy = null;
            }
        }
        if (isEnemy && collision.CompareTag("Player"))
        {
            if (OnHit != null)
            {
                OnHit(4, collision.gameObject.GetInstanceID());
                OnHit = null;
            }
            if (OnDestroy != null)
            {
                OnDestroy();
                OnDestroy = null;
            }
        }
    }
    public virtual void OnUpdate()
    {

    }
    public virtual void OnReset()
    {
        Debug.Log("base");
        this.isSetUp = false;
        this.target = null;
        this.targetPos = Vector3.zero;
        this.lifeTime = 0;
    }
}
