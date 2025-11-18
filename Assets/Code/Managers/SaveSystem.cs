using System.IO;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public class SaveData
    {
        public float[] checkpointPosition;

        public SaveData(Vector3 gamePosition)
        {
            checkpointPosition = new float[3];
            checkpointPosition[0] = gamePosition.x;
            checkpointPosition[1] = gamePosition.y;
            checkpointPosition[2] = gamePosition.z;
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

    public static void WriteSaveData(Vector3 gamePosition, int id)
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