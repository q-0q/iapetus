using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveSystem : MonoBehaviour
{
    
    private static SaveSystem _singleton;
    private static SaveSystem Singleton
    {
        get
        {
            if (_singleton == null)
            {
                var go = new GameObject("SaveSystem");
                _singleton = go.AddComponent<SaveSystem>();
            }
            return _singleton;
        }
    }
    
    void Awake()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        _cachedSaveData = null;
        _singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    
    [System.Serializable]
    public class TrialCompletionEntry
    {
        public string metaName;
        public float time;
    }

    
    [System.Serializable]
    public class SaveData
    {
        public float[] playerInGamePosition;
        public string playerInGamePositionId;
        public float playerInGameYAngle;
        public List<string> persistentEvents;
        public List<TrialCompletionEntry> trialCompletions;
        public List<string> lemonCollections;
        public List<string> bells;
        public int bitCount;
        public List<string> bitDeposits;
            

        public SaveData()
        {
            playerInGamePosition = null;
            playerInGameYAngle = 0f;
            playerInGamePositionId = "";
            persistentEvents = new List<string>();
            trialCompletions = new List<TrialCompletionEntry>();
            lemonCollections = new List<string>();
            bells = new List<string>();
            bitCount = 0;
            bitDeposits = new List<string>();
        }
    }

    private SaveData _cachedSaveData;
    
    private static string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath, "saves");
    }

    private static string GetPath(int id)
    {
        return Path.Combine(GetDirectory(), id + ".json");
    }
    
    public static void WritePlayerInGamePosition(Vector3 gamePosition, string gamePositionId, float yAngle, int id)
    {
        SaveData data = LoadSaveData(id);
        data.playerInGamePosition = new []{ gamePosition.x, gamePosition.y, gamePosition.z};
        data.playerInGamePositionId = gamePositionId;
        data.playerInGameYAngle = yAngle;
        WriteSaveData(data, id);
    }
    
    public static void WritePersistentEvent(string persistentEvent, int id)
    {
        SaveData data = LoadSaveData(id);
        if (data.persistentEvents.Contains(persistentEvent)) return;
        data.persistentEvents.Add(persistentEvent);
        WriteSaveData(data, id);
    }
    
    public static void WriteBell(string metaName, int id)
    {
        SaveData data = LoadSaveData(id);
        if (data.bells.Contains(metaName)) return;
        data.bells.Add(metaName);
        WriteSaveData(data, id);
    }

    public static void WriteTrialCompletion(string metaName, float time, int id)
    {
        SaveData data = LoadSaveData(id);

        var entry = data.trialCompletions.FirstOrDefault(e => e.metaName == metaName);

        if (entry != null)
        {
            if (entry.time < time) return;
            entry.time = time;
        }
        else
        {
            data.trialCompletions.Add(new TrialCompletionEntry
            {
                metaName = metaName,
                time = time
            });
        }

        WriteSaveData(data, id);
    }

    
    public static bool GetTrialCompletion(string metaName, out float playerRecordTime, int id)
    {
        playerRecordTime = -1f;
        SaveData data = LoadSaveData(id);
        var entry = data.trialCompletions.FirstOrDefault(e => e.metaName == metaName);
        if (entry != null) playerRecordTime = entry.time;
        return entry != null;
    }
    
    public static void WriteLemonCollection(string metaName, int id)
    {
        SaveData data = LoadSaveData(id);
        var entry = data.lemonCollections.FirstOrDefault(e => e == metaName);
        if (entry != null) return;
        data.lemonCollections.Add(metaName);
        WriteSaveData(data, id);
    }
    
    public static void AddBit(int id)
    {
        SaveData data = LoadSaveData(id);
        data.bitCount++;
        Singleton._cachedSaveData = data;
    }
    
    public static void CollectBitDeposit(string metaName, int id)
    {
        SaveData data = LoadSaveData(id);
        var entry = data.bitDeposits.FirstOrDefault(e => e == metaName);
        if (entry != null) return;
        data.bitDeposits.Add(metaName);
        Singleton._cachedSaveData = data;
    }
    
    public static bool GetBitDeposit(string metaName, int id)
    {
        SaveData data = LoadSaveData(id);
        var entry = data.bitDeposits.FirstOrDefault(e => e == metaName);
        return entry != null;
    }
    
    public static bool GetLemonCollection(string metaName, int id)
    {
        SaveData data = LoadSaveData(id);
        var entry = data.lemonCollections.FirstOrDefault(e => e == metaName);
        return entry != null;
    }

    public static bool GetPersistentEventCompleted(string persistentEvent)
    {
        var data = LoadSaveData(0);
        return data.persistentEvents.Contains(persistentEvent);
    }
    
    public static bool GetBell(string metaName)
    {
        var data = LoadSaveData(0);
        return data.bells.Contains(metaName);
    }
    
    private static void WriteSaveData(SaveData saveData, int id)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        SaveData data = saveData;
        Singleton._cachedSaveData = data;
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(id), json);
        Debug.Log("Saved file to: " + GetPath(id));
    }

    public static SaveData LoadSaveData(int id)
    {
        if (Singleton._cachedSaveData == null)
        {
            string path = GetPath(id);
            if (!File.Exists(path))
            {
                Singleton._cachedSaveData = new SaveData();
            }
            else
            {
                string json = File.ReadAllText(path);
                Singleton._cachedSaveData = JsonUtility.FromJson<SaveData>(json);
            }
        };
        
        return Singleton._cachedSaveData;
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