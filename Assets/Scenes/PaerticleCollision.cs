using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaerticleCollision : MonoBehaviour 
{
    private ParticleSystem _particle;
    private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

    private void Start()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject obj)
	{
		if(obj.tag == "water")
		{   
            //　イベントの取得
            _particle.GetCollisionEvents(obj, _collisionEvents);
            List<Ray> rays = new List<Ray>();

            //　衝突した位置を取得
            foreach (var colEvent in _collisionEvents)
            {
                rays.Add(new Ray(colEvent.intersection,colEvent.velocity.normalized));
            }

            if(rays.Count != 0)
            obj.GetComponent<WaterSimulation>().OnCollision(rays);
        }
	}
}
