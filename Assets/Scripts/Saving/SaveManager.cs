using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private static SaveManager instance = null;
    private const string PlayerPrefsKey = "SaveData";

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

private bool IsWebBuild()
    {
#if UNITY_WEBGL
        return true;
#else
        return false;
#endif
    }

    private void LoadData()
    {
        if (IsWebBuild())
        {
            LoadDataFromPlayerPrefs();
        }
        else
        {
            LoadDataFromFile();
        }
    }

    private void SaveData()
    {
        if (IsWebBuild())
        {
            SaveDataToPlayerPrefs();
        }
        else
        {
            SaveDataToFile();
        }
    }



    private void LoadDataFromFile()
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
    private void SaveDataToFile()
    {
        string json = JsonUtility.ToJson(State, true);
        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
    }

    private void LoadDataFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            State = JsonUtility.FromJson<SaveState>(json);
        }
        else
        {
            State = SaveState.Defaults();
        }

    }

    private void SaveDataToPlayerPrefs()
    {
        string json = JsonUtility.ToJson(State, false);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }


    public void Save(SaveState newData)
    {
        State = newData;
        SaveData();
    }
}
