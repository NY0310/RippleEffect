using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaerticleCollision : MonoBehaviour 
{
    private ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }
    void OnParticleCollision(GameObject obj)
	{
		if(obj.tag == "water")
		{


            
            //　イベントの取得
            particle.GetCollisionEvents(obj, collisionEvents);
            List<Ray> rays = new List<Ray>();


            //　衝突した位置を取得し、ダメージスクリプトを呼び出す
            foreach (var colEvent in collisionEvents)
            {
                rays.Add(new Ray(colEvent.intersection,colEvent.velocity.normalized));
                //Debug.Log("パーティクル座標" + pos);
            }
            Debug.Log(rays.Count);
            if(rays.Count != 0)
            obj.GetComponent<WaterSimulation>().OnCollision(rays);
        }
	}
}
