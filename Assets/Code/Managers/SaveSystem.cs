using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

public static class SaveSystem
{

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

    public static void WriteSaveData(PlayerFsm playerFsm, int id)
    {
        var formatter = new BinaryFormatter();
        var path = System.IO.Path.Combine(Application.persistentDataPath, "saves", id.ToString());
        var stream = new FileStream(path, FileMode.Create);
        var saveData = new SaveData(playerFsm);
        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static SaveData LoadSaveData(int id)
    {
        
        var path = System.IO.Path.Combine(Application.persistentDataPath, "saves", id.ToString());
        if (!File.Exists(path))
        {
            Debug.LogError("LoadSaveData failed: no file found at " + path);
            return null;
        }
        
        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Open);
        var saveData = formatter.Deserialize(stream) as SaveData;
        stream.Close();
        return saveData;
    }
}
