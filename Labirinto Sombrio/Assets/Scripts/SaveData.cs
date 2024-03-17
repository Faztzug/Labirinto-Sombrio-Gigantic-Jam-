using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum GameDificulty
{
    Easy,
    Normal,
    Hard,
}

public class SaveManager
{
    private const string kSavePath = "/MySaveData.dat";
    public void SaveGame(SaveData saveData)
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath + kSavePath); 
        bf.Serialize(file, saveData);
        file.Close();
        //Debug.Log("Game data saved!");
    }
    public SaveData LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + kSavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + kSavePath, FileMode.Open);
            SaveData loadData = (SaveData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
            return loadData;
        }
        else
        {
            Debug.LogError("There is no save data!");
            return new SaveData();
        }
    }
    
    public SaveData ResetData()
    {
        if (File.Exists(Application.persistentDataPath + kSavePath))
        {
            File.Delete(Application.persistentDataPath + kSavePath);
            Debug.Log("Data reset complete!");
        }
        else
        Debug.LogError("No save data to delete.");
        return new SaveData();
    }
}

[Serializable]
public class SaveData
{
    [Header("GameData")]
    public GameDificulty gameDificulty;

    [Header("Cutscene")]
    public bool jumpCutscene;

    public SaveData()
    {
        gameDificulty = GameDificulty.Normal;
    }
}
