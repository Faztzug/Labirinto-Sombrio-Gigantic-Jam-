using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0, 1)] [SerializeField]
    private float volume = 1f;
    public float SFXVolume => GameState.SettingsData.mute ? 0f : volume * GameState.SettingsData.sfxVolume;
    public float MusicVolume => volume * GameState.SettingsData.musicVolume;
    [Range(-3, 3)]
    public float pitch = 1f;
    [Range(-3, 3)]
    public float[] pitchRNG = new float[2]{0,0};
    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource audioSource;
    public bool IsPlaying => audioSource != null && audioSource.isPlaying && audioSource.clip == clip;
    
    public void Setup(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        audioSource.clip = clip;
        audioSource.volume = SFXVolume;
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.playOnAwake = playOnAwake;
        if(loop)
        {
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = GameState.AudioMixer.FindMatchingGroups("SFX")[0];
        }
    }

    public void PlayOn(AudioSource audioSource, bool oneShot = true)
    {
        //if(GameState.GameStateInstance != null && GameState.SaveData.mute) return;
        
        Setup(audioSource);
        if(pitchRNG.Length >= 2 && pitchRNG[0] != pitchRNG[1])
        {
            this.audioSource.pitch = pitch + Random.Range(pitchRNG[0], pitchRNG[1]);
        }
        if(clip) 
        {
            if(oneShot) this.audioSource.PlayOneShot(clip);
            else this.audioSource.Play();
        }
    }

}
