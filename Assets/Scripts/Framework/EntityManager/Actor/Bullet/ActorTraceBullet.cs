using UnityEngine;

namespace CaomaoFramework
{
    public class ActorTraceBullet : ActorBullet
    {
        public override void OnUpdate()
        {
            if (target != null)
            {
                targetPos = target.position;
            }
            transform.LookAt(targetPos);
            float step = speed * Time.deltaTime;
            float distance = Vector3.Distance(targetPos, transform.position);
            if (distance < step)
            {
                //到达目标
                transform.position = targetPos;
                if (OnDestroy != null)
                    OnDestroy();
            }
            else
            {
                if (!isEnemy)
                    transform.Translate(step * transform.right, Space.World);
                else
                    transform.Translate(step * -transform.right, Space.World);
            }
        }
    }
}
