using UnityEngine;

namespace CaomaoFramework
{
    public class ActorLineBullet : ActorBullet
    {
        private float time = 0;
        public override void OnUpdate()
        {
            float step = speed * Time.deltaTime;
            time += Time.deltaTime;
            if (!isEnemy)
            {
                transform.Translate(step * transform.right, Space.World);
            }
            else
            {
                transform.Translate(step * -transform.right, Space.World);
            }
            if (time >= lifeTime)
            {
                if (OnDestroy != null)
                {
                    this.OnDestroy();
                }
            }
        }
        public override void OnReset()
        {
            Debug.Log("line");
            base.OnReset();
            this.time = 0;
        }
    }  
}
