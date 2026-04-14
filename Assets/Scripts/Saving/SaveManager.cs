using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private static SaveManager instance = null;

    public static SaveManager Instance{ 
        get {
            instance ??= new SaveManager();

            return instance;
        }
    }

    private SaveManager()
    {
        LoadData();
    }

    public SaveState State { get; private set; }

    private void LoadData()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            State = JsonUtility.FromJson<SaveState>(json);
        } else
        {
            State = SaveState.Defaults();
        }
    }
    private void SaveData()
    {
        string json = JsonUtility.ToJson(State, true);
        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
    }

    public void Save(SaveState newData)
    {
        State = newData;
        SaveData();
    }
}
