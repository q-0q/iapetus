using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public static class SaveSystem
{
    
    
    [System.Serializable]
    public class SaveData
    {
        public float[] playerInGamePosition;
        public float playerInGameYAngle;

        public SaveData()
        {
            playerInGamePosition = null;
            playerInGameYAngle = 0f;
        }
    }
    
    private static string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath, "saves");
    }

    private static string GetPath(int id)
    {
        return Path.Combine(GetDirectory(), id + ".json");
    }
    
    public static void WritePlayerInGamePosition(float[] gamePosition, float yAngle, int id)
    {
        SaveData data = LoadSaveData(id);
        data.playerInGamePosition = gamePosition;
        data.playerInGameYAngle = yAngle;
        WriteSaveData(data, id);
    }

    private static void WriteSaveData(SaveData saveData, int id)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        SaveData data = saveData;
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(id), json);
        Debug.Log("Saved file to: " + GetPath(id));
    }

    public static SaveData LoadSaveData(int id)
    {
        string path = GetPath(id);
        if (!File.Exists(path))
        {
            return new SaveData();
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }
}

public static class MetaSaveSystem
{
    public static event Action<MetaSaveSystem.MetaSaveData> OnMetaSaveDataUpdated;
        
    [System.Serializable]
    public class MetaSaveData
    {

        public int saveId;
        public float cameraSensitivityModifier;
        public bool enableAmbientParticles;
        public bool enableFpsDisplay;
        
        public MetaSaveData(int saveId, float cameraSensitivityModifier, bool enableAmbientParticles, bool enableFpsDisplay)
        {
            this.saveId = saveId;
            this.cameraSensitivityModifier = cameraSensitivityModifier;
            this.enableAmbientParticles = enableAmbientParticles;
            this.enableFpsDisplay = enableFpsDisplay;
        }
    }
    
    private static string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath);
    }

    private static string GetPath()
    {
        return Path.Combine(GetDirectory(), "meta.json");
    }

    public static MetaSaveData WriteMetaSaveData(int saveId, float cameraSensitivityModifier, bool enableAmbientParticles, bool enableFpsDisplay)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        MetaSaveData data = new MetaSaveData(saveId, cameraSensitivityModifier, enableAmbientParticles, enableFpsDisplay);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), json);
        Debug.Log("Saved file to: " + GetPath());
        OnMetaSaveDataUpdated?.Invoke(data);
        return data;
    }

    public static MetaSaveData LoadMetaSaveData()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            return WriteDefaultMetaSaveData();
        }

        string json = File.ReadAllText(path);
        MetaSaveData data = JsonUtility.FromJson<MetaSaveData>(json);
        OnMetaSaveDataUpdated?.Invoke(data);
        return data;
    }

    public static MetaSaveData WriteDefaultMetaSaveData()
    {
        return WriteMetaSaveData(0, 1f, true, true);
    }
}