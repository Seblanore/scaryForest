using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundRandomiser : NetworkBehaviour
{
    public AudioClip[] idleClips;
    public AudioClip[] chasingClips;
    public AudioClip[] attackingClips;

    private AudioSource source;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!source.isPlaying)
        {
            if(animator.GetBool("chase"))
            {
                source.clip = chasingClips[Random.Range(0, chasingClips.Length)];
                source.volume = Random.Range(0.8f, 0.9f);
                source.pitch = Random.Range(0.8f, 1f);
            } else if(animator.GetBool("attack"))
            {
                source.clip = attackingClips[Random.Range(0, attackingClips.Length)];
                source.volume = Random.Range(0.9f, 1f);
                source.pitch = Random.Range(0.8f, 1f);
            } else
            {
                source.clip = idleClips[Random.Range(0, idleClips.Length)];
                source.volume = Random.Range(0.4f, 0.6f);
                source.pitch = Random.Range(0.8f, 1f);
            }
            source.PlayOneShot(source.clip);
        }  
    }
}
