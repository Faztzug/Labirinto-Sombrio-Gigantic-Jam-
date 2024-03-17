using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IddleSound : MonoBehaviour
{
    [SerializeField] private Sound sound;
    [SerializeField] bool renewLoop;
    [SerializeField] Vector2 renewTimerRNG;
    private float timer;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if(renewLoop) sound.loop = false;
        timer = Random.Range(renewTimerRNG.x, renewTimerRNG.y);
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
                timer = Random.Range(renewTimerRNG.x, renewTimerRNG.y);
            }
        }
    }
}
