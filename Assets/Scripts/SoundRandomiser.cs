using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandomiser : MonoBehaviour
{
    public AudioClip[] idleClips;
    public AudioClip[] chasingClips;
    public AudioClip[] attackingClips;

    private AudioSource source;
    private Animator animator;

    private void Start()
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
