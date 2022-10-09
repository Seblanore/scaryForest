using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        /*source.Stop();
        source.clip = damagedClips[Random.Range(0, damagedClips.Length)];
        source.Play();*/

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Disconnect();
        SceneManager.LoadScene(2);
    }

    void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
