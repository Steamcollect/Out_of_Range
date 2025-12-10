using UnityEngine;
using System;
using System.IO;

public static class SavingSystem
{
    // Events pour notifier les changements
    public static event Action OnSaveCompleted;
    public static event Action OnLoadCompleted;
    public static event Action OnDeleteCompleted;
    public static event Action<SaveData> OnDataChanged;
    
    private static string s_Filepath;
    private static SaveData s_CurrentData;

    public static SaveData CurrentData
    {
        get
        {
            if (s_CurrentData == null)
            {
                Load();
            }
            return s_CurrentData;
        }
        set
        {
            s_CurrentData = value;
            OnDataChanged?.Invoke(s_CurrentData);
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        s_CurrentData = null;
        
        OnSaveCompleted = null;
        OnLoadCompleted = null;
        OnDeleteCompleted = null;
        
        OnDataChanged = null;

        s_Filepath = SettingFilepath();
    }

    public static bool Save()
    {
        try
        {
            CurrentData.SaveDataInfo.LastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            string jsonData = JsonUtility.ToJson(CurrentData, true);
            File.WriteAllText(s_Filepath, jsonData);
            
            OnSaveCompleted?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"{typeof(SavingSystem)} Error Saving: {e.Message}");
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (!FileExists())
            {
                CurrentData = new SaveData();
                return false;
            }
            
            string jsonData = File.ReadAllText(s_Filepath);
            CurrentData = JsonUtility.FromJson<SaveData>(jsonData);
            
            OnLoadCompleted?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"{typeof(SavingSystem)} Error Loading: {e.Message}");
            CurrentData = new SaveData();
            return false;
        }
    }

    public static bool Delete()
    {
        try
        {
            if (FileExists())
            {
                File.Delete(s_Filepath);
            } 
            CurrentData = null;
            OnDeleteCompleted?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"{typeof(SavingSystem)} Error delete: {e.Message}");
            return false;
        }
    }


    private static bool FileExists()
    {
        if (string.IsNullOrEmpty(s_Filepath))
        {
            s_Filepath = SettingFilepath();
        }
        return File.Exists(s_Filepath);
    }
    
    private static string SettingFilepath()
    {
        return Path.Combine(Application.persistentDataPath, $"{Application.productName}_Save.json");
    }
}
