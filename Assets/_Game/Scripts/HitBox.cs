using Unity.Netcode;
using UnityEngine;

public class HitBox : NetworkBehaviour
{
    public Health health;

    [ClientRpc]
    public void OnHitClientRpc(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float damage;
        switch (tag)
        {
            case "limb":
                damage = 5f;
                break;
            case "head":
                damage = 30f;
                break;
            case "body":
                damage = 10f;
                break;

            default:
                damage = 5f;
                break;
        }
        health.TakeDamage(damage, direction, rb);
        
    }
}
