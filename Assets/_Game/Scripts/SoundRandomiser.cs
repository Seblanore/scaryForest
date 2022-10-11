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
        if (!source.isPlaying)
        {
            if(animator.GetBool("chase"))
            {
                source.clip = chasingClips[Random.Range(0, chasingClips.Length)];
            } else if(animator.GetBool("attack"))
            {
                source.clip = attackingClips[Random.Range(0, attackingClips.Length)];
            } else
            {
                source.clip = idleClips[Random.Range(0, idleClips.Length)];
            }
            source.PlayOneShot(source.clip);
        }  
    }
}
