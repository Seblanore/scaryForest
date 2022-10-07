using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Health health;

    public void OnHit(Vector3 direction)
    {
        health.TakeDamage(10, direction);
    }
}
