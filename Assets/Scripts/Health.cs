
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float dieForce;
    public float maxHealth;
    public float currentHealth;
    Ragdoll ragdoll;

    public AudioClip[] damagedClips;
    private AudioSource source;

    private bool isAlive = true;


    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        source = GetComponent<AudioSource>();

        currentHealth = maxHealth;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
        }
    }

    public void TakeDamage(float amount, Vector3 direction, Rigidbody rb)
    {
        if(isAlive)
        {
            currentHealth -= amount;
            source.Stop();
            source.clip = damagedClips[Random.Range(0, damagedClips.Length)];
            source.Play();

            if (currentHealth <= 0.0f)
            {
                Die(direction, rb);
            }
        } else
        {
            direction.y = 1;
            ApplyForce(rb, direction * dieForce);
        }  
    }

    private void Die(Vector3 direction, Rigidbody rb)
    {
        ragdoll.ActivateRagdoll();
        direction.y = 1;
        ApplyForce(rb, direction * dieForce);

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<ZombieAi>());
        Destroy(GetComponent<SoundRandomiser>());
        Destroy(GetComponent<AudioSource>());
        isAlive = false;
    }

    public void ApplyForce(Rigidbody rb, Vector3 force)
    {

        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
