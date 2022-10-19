using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletProjectile : NetworkBehaviour
{
    [SerializeField] private Transform blood;

    float m_DestroyAtSec;

public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                m_DestroyAtSec = Time.fixedTime + 2;
            }
        }

    void FixedUpdate() {
        if (m_DestroyAtSec < Time.fixedTime)
            {
                // Time to return to the pool from whence it came.
                var networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Despawn();
                return;
            }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        /*if(other.material.name == "Zombie Flesh (Instance)")
        {
            Instantiate(blood, transform.position, Quaternion.LookRotation(-transform.forward));
        }
     
        var hitBox = other.GetComponent<HitBox>();
        if(hitBox)
        {
            hitBox.OnHit(transform.forward);
        }
        Destroy(gameObject);*/
    }
}
