using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public static class SaveSystem
{
    [System.Serializable]
    public class SaveData
    {
        public float[] playerInGamePosition;

        public SaveData(float[] gamePosition)
        {
            playerInGamePosition = new float[3];
            if (gamePosition == null)
            {
                playerInGamePosition = null;
                return;
            }
            playerInGamePosition[0] = gamePosition[0];
            playerInGamePosition[1] = gamePosition[1];
            playerInGamePosition[2] = gamePosition[2];
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

    public static void WriteSaveData(float[] gamePosition, int id)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        SaveData data = new SaveData(gamePosition);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(id), json);
        Debug.Log("Saved file to: " + GetPath(id));
    }

    public static SaveData LoadSaveData(int id)
    {
        string path = GetPath(id);
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }
}

public static class MetaSaveSystem
{
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

    public static void WriteMetaSaveData()
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        MetaSaveData data = new MetaSaveData(0, 1f, true, true);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), json);
        Debug.Log("Saved file to: " + GetPath());
    }

    public static MetaSaveData LoadMetaSaveData()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        MetaSaveData data = JsonUtility.FromJson<MetaSaveData>(json);
        return data;
    }
}