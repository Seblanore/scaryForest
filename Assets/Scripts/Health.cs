using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float dieForce;
    public float maxHealth;
    public float currentHealth;
    Ragdoll ragdoll;


    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        currentHealth = maxHealth;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
        }
    }

    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;
        if(currentHealth <= 0.0f)
        {
            Die(direction);
        }
    }

    private void Die(Vector3 direction)
    {
        ragdoll.ActivateRagdoll();
        direction.y = 1;
        ragdoll.ApplyForce(direction * dieForce);

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<ZombieAi>());
    }
}
