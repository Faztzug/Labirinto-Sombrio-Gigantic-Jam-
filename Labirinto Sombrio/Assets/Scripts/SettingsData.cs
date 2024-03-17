using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[Serializable]
public enum Quality
{
    Low,
    Medium,
    High,
}

[Serializable]
public class SettingsData
{
    [Header("Settings")]
    public bool mute;
    public float sfxVolume;
    public float musicVolume;
    public Quality quality;
    public int FPS;
    public bool showFPS;
    public float sensibilidadeX;
    public float sensibilidadeY;

    public SettingsData()
    {
        mute = false;
        musicVolume = 0.8f;
        sfxVolume = 0.8f;
        quality = Quality.High;
        FPS = 200;
        showFPS = false;
        sensibilidadeX = 0.5f;
        sensibilidadeY = 0.5f;
    }
}

public class SettingsDataManager
{
    private const string kSettingsPath = "/MySettingsData.dat";
    public void SaveSettings(SettingsData settingsData)
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + kSettingsPath); 
        bf.Serialize(file, settingsData);
        file.Close();
        //Debug.Log("Game data saved!");
    }
    public SettingsData LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + kSettingsPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + kSettingsPath, FileMode.Open);
            SettingsData loadData = (SettingsData)bf.Deserialize(file);
            file.Close();
            //Debug.Log("Game data loaded!");
            return loadData;
        }
        else
        {
            Debug.LogError("There is no settings data!");
            SaveSettings(new SettingsData());
            return new SettingsData();
        }
    }
    
    public SettingsData ResetData()
    {
        if (File.Exists(Application.persistentDataPath + kSettingsPath))
        {
            File.Delete(Application.persistentDataPath + kSettingsPath);
            Debug.Log("Data reset complete!");
        }
        else
        Debug.LogError("No save data to delete.");
        return new SettingsData();
    }
}
