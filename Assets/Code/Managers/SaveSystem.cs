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
                Debug.Log("It's null baby!");
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