using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IddleSound : MonoBehaviour
{
    [SerializeField] private Sound sound;
    [SerializeField] bool renewLoop;
    [SerializeField] float renewTimer;
    private float timer;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if(renewLoop) sound.loop = false;
        timer = renewTimer;
        Play();
    }

    public void Play()
    {
        if(!audioSource) audioSource = GetComponentInChildren<AudioSource>();
        sound.PlayOn(audioSource, false);
    }

    private void Update()
    {
        if (!sound.IsPlaying & renewLoop)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                sound.PlayOn(audioSource, false);
                timer = renewTimer;
            }
        }
    }
}
