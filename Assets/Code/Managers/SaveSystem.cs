using System.IO;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public class SaveData
    {
        public float[] checkpointPosition;

        public SaveData(PlayerFsm playerFsm)
        {
            checkpointPosition = new float[3];
            checkpointPosition[0] = playerFsm.transform.position.x;
            checkpointPosition[1] = playerFsm.transform.position.y;
            checkpointPosition[2] = playerFsm.transform.position.z;
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

    public static void WriteSaveData(PlayerFsm playerFsm, int id)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        SaveData data = new SaveData(playerFsm);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(id), json);
        Debug.Log("Saved file to: " + GetPath(id));
    }

    public static SaveData LoadSaveData(int id)
    {
        string path = GetPath(id);
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save not found at " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }
}