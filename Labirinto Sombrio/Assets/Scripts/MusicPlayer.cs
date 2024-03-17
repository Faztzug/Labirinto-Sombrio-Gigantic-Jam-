using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private Sound musicSound;
    private AudioClip CurrentPlaying;
    private AudioSource audioSource;
    public AudioSource GetAudioSource => audioSource;

    public void ChangeMusic(Sound newMusic)
    {
        musicSound = newMusic;
        SettingMusic();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SettingMusic();
        if (GameState.GameStateInstance != null) GameState.OnSettingsUpdated += UpdateVolume;
    }

    private void SettingMusic()
    {
        musicSound.Setup(audioSource);
        audioSource.outputAudioMixerGroup = null;

        UpdateVolume();

        audioSource.Play();
        CurrentPlaying = musicSound.clip;
    }

    public void UpdateVolume()
    {
        if(this == null) return;
        if(GameState.GameStateInstance != null)
        {
            audioSource.volume = GameState.SettingsData.mute ? 0f : musicSound.MusicVolume;
        }
    }

    private void OnDestroy() 
    {
        GameState.OnSettingsUpdated -= UpdateVolume;
    }
}
