using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle mute;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;
    [SerializeField] private TMP_Dropdown quality;
    //[SerializeField] private Slider fps;
    //[SerializeField] private Toggle showFps;
    [SerializeField] private Slider sensibilidadeX;
    [SerializeField] private Slider sensibilidadeY;

    private SettingsDataManager settingsManager = new SettingsDataManager();
    private SettingsData settingsData;
    private SettingsData SettingsData {get => settingsData; 
    set {settingsData = value;}}


    private void Start() 
    {
        SettingsData = settingsManager.LoadSettings();
        mute.isOn = SettingsData.mute;
        music.value = SettingsData.musicVolume;
        sfx.value = SettingsData.sfxVolume;
        quality.value = (int)SettingsData.quality;
        //showFps.isOn = SettingsData.showFPS;
        //fps.value = settingsData.FPS;
        sensibilidadeX.value = settingsData.sensibilidadeX;
        sensibilidadeY.value = settingsData.sensibilidadeY;

        if(GameState.GameStateInstance) GameState.OnSettingsUpdated += SettingsHasUpdated;

        mute.onValueChanged.AddListener(MuteChanged);
        music.onValueChanged.AddListener(MusicChanged);
        sfx.onValueChanged.AddListener(SFXChanged);
        quality.onValueChanged.AddListener(QualityChanged);
        //showFps.isOn = SettingsData.showFPS;
        //fps.value = settingsData.FPS;
        sensibilidadeX.onValueChanged.AddListener(SensibilidadeXChanged);
        sensibilidadeY.onValueChanged.AddListener(SensibilidadeYChanged);
    }

    public void Close()
    {
        closeButton.onClick.Invoke();
    }
    
    private void UpdateFileData()
    {
        settingsManager.SaveSettings(SettingsData);
        GameState.settingsManager.SaveSettings(SettingsData);
        if (SettingsData.mute)
        {
            GameState.AudioMixer.SetFloat("music", Mathf.Log10(0.0001f) * 20);
            GameState.AudioMixer.SetFloat("sfx", Mathf.Log10(0.0001f) * 20);
        }
        else
        {
            GameState.AudioMixer.SetFloat("music", Mathf.Log10(SettingsData.musicVolume + 0.0001f) * 20);
            GameState.AudioMixer.SetFloat("sfx", Mathf.Log10(SettingsData.sfxVolume + 0.0001f) * 20);
        }
        if (GameState.GameStateInstance) GameState.OnSettingsUpdated?.Invoke();
    }
    public void MuteChanged(bool value)
    {
        SettingsData.mute = value;
        UpdateFileData();
    }
    public void MusicChanged(float value)
    {
        SettingsData.musicVolume = value;
        UpdateFileData();
    }
    public void SFXChanged(float value)
    {
        SettingsData.sfxVolume = value;
        UpdateFileData();
    }
    public void QualityChanged(int value)
    {
        SettingsData.quality = (Quality)value;
        UpdateFileData();
    }
    public void ShowFPSChanged(bool value)
    {
        SettingsData.showFPS = value;
        UpdateFileData();
    }
    public void SensibilidadeXChanged(float value)
    {
        SettingsData.sensibilidadeX = value;
        UpdateFileData();
    }
    public void SensibilidadeYChanged(float value)
    {
        SettingsData.sensibilidadeY = value;
        UpdateFileData();
    }

    private void SettingsHasUpdated()
    {
        if(GameState.GameStateInstance) GameState.UpdateQuality();
    }

    private void OnDestroy() 
    {
        if(GameState.GameStateInstance) GameState.OnSettingsUpdated -= SettingsHasUpdated;
    }
}
