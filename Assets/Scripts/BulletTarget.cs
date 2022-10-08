using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletTarget : NetworkBehaviour
{
    [SerializeField] private Transform blood;

    private Rigidbody bulletRigidbody;

    public override void OnNetworkSpawn()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.material.name == "Zombie Flesh (Instance)")
        {
            Instantiate(blood, transform.position, Quaternion.LookRotation(-transform.forward));
        }
        
        other.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position, 10);
        var hitBox = other.GetComponent<HitBox>();
        if(hitBox)
        {
            hitBox.OnHit(transform.forward);
        }
    }
}
