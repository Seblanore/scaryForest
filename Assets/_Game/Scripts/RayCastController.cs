using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RayCastController : NetworkBehaviour
{
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private Transform blood;

    [ServerRpc]
    public void ShootServerRpc(Vector3 position, Vector3 direction)
    {
        Transform hitTransform = null;
        Collider collider = null;
        Ray ray = new Ray(position, direction);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            hitTransform = raycastHit.transform;
            collider = raycastHit.collider;
        }

        if (collider && hitTransform)
        {
            if (collider.material.name == "Zombie Flesh (Instance)")
            {
                BloodExplosionClientRpc(hitTransform.position, direction);
            }

            var hitBox = collider.GetComponent<HitBox>();
            if (hitBox)
            {
                hitBox.OnHitClientRpc(direction);
            }
        }
    }

    [ClientRpc]
    public void BloodExplosionClientRpc(Vector3 position, Vector3 direction)
    {
        Transform t = Instantiate(blood, position, Quaternion.LookRotation(-direction));
        Destroy(t.gameObject, 2);
    }


}
