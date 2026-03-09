using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SaveSystem : MonoBehaviour
{

    private float _timeSinceLastSave;
    
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
        _timeSinceLastSave = 0;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        _timeSinceLastSave += Time.deltaTime;
    }

    [System.Serializable]
    public class TrialCompletionEntry
    {
        public string metaName;
        public float time;
        public float goldTime;
    }

    
    [System.Serializable]
    public class SaveData
    {
        public string scene;
        public float[] playerInGamePosition;
        public string playerInGamePositionId;
        public float playerInGameYAngle;
        public List<string> persistentEvents;
        public List<TrialCompletionEntry> trialCompletions;
        public List<string> lemonCollections;
        public List<string> bells;
        public int bellCount;
        public int bitCount;
        public List<string> bitDeposits;
        public float playTime;
            

        public SaveData()
        {
            scene = "C1-Brazier";
            playerInGamePosition = null;
            playerInGameYAngle = 0f;
            playerInGamePositionId = "";
            persistentEvents = new List<string>();
            trialCompletions = new List<TrialCompletionEntry>();
            lemonCollections = new List<string>();
            bells = new List<string>();
            bitCount = 0;
            bellCount = 0;
            playTime = 0;
            bitDeposits = new List<string>();
        }
    }

    private SaveData _cachedSaveData;
    
    private static string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath, "saves");
    }

    private static string GetPath()
    {
        var id = MetaSaveSystem.LoadMetaSaveData().saveId;
        return Path.Combine(GetDirectory(), id + ".json");
    }
    
    public static void WritePlayerInGamePosition(Vector3 gamePosition, string gamePositionId, float yAngle)
    {
        SaveData data = LoadCachedSaveData();
        data.playerInGamePosition = new []{ gamePosition.x, gamePosition.y, gamePosition.z};
        data.playerInGamePositionId = gamePositionId;
        data.playerInGameYAngle = yAngle;
        WriteSaveData(data);
    }
    
    public static void ClearPlayerInGamePosition()
    {
        SaveData data = LoadCachedSaveData();
        data.playerInGamePosition = null;
        data.playerInGamePositionId = "";
        data.playerInGameYAngle = 0;
        WriteSaveData(data);
    }
    
    public static void WritePersistentEvent(string persistentEvent)
    {
        if (persistentEvent == "")
        {
            Debug.LogError("Tried to write empty persistent event");
            return;
        }
        SaveData data = LoadCachedSaveData();
        if (data.persistentEvents.Contains(persistentEvent)) return;
        data.persistentEvents.Add(persistentEvent);
        WriteSaveData(data);
    }
    
    public static void WriteBell(string metaName)
    {
        if (metaName == "")         
        {
            Debug.LogError("Tried to write empty bell");
            return;
        }
        SaveData data = LoadCachedSaveData();
        if (data.bells.Contains(metaName)) return;
        data.bellCount++;
        data.bells.Add(metaName);
        WriteSaveData(data);
    }
    
    public static void ReduceBellCount(int amount)
    {
        SaveData data = LoadCachedSaveData();
        data.bellCount -= amount;
        WriteSaveData(data);
    }

    public static void WriteTrialCompletion(string metaName, float time, float goldTime)
    {
        if (metaName == "")
        {
            Debug.LogError("Tried to write empty trial");
            return;
        };
        
        SaveData data = LoadCachedSaveData();

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
                time = time,
                goldTime = goldTime
            });
        }

        WriteSaveData(data);
    }

    
    public static bool GetTrialCompletion(string metaName, out float playerRecordTime)
    {
        playerRecordTime = -1f;
        SaveData data = LoadCachedSaveData();
        var entry = data.trialCompletions.FirstOrDefault(e => e.metaName == metaName);
        if (entry != null) playerRecordTime = entry.time;
        return entry != null;
    }
    
    public static bool GetTrialGolded(string metaName)
    {
        SaveData data = LoadCachedSaveData();
        var entry = data.trialCompletions.FirstOrDefault(e => e.metaName == metaName);
        if (entry == null) return false;
        return entry.time < entry.goldTime;
    }
    
    public static void WriteLemonCollection(string metaName)
    {
        if (metaName == "")
        {
            Debug.LogError("Tried to write empty lemon");
            return;
        };
        
        SaveData data = LoadCachedSaveData();
        var entry = data.lemonCollections.FirstOrDefault(e => e == metaName);
        if (entry != null) return;
        data.lemonCollections.Add(metaName);
        WriteSaveData(data);
    }
    
    public static void AddBit()
    {
        SaveData data = LoadCachedSaveData();
        data.bitCount++;
        Singleton._cachedSaveData = data;
    }
    
    public static void RemoveBit(int amount)
    {
        SaveData data = LoadCachedSaveData();
        data.bitCount -= amount;
        Singleton._cachedSaveData = data;
    }
    
    public static void CollectBitDeposit(string metaName)
    {
        if (metaName == "") 
        {
            Debug.LogError("Tried to write empty bit deposit");
            return;
        };
        
        SaveData data = LoadCachedSaveData();
        var entry = data.bitDeposits.FirstOrDefault(e => e == metaName);
        if (entry != null) return;
        data.bitDeposits.Add(metaName);
        Singleton._cachedSaveData = data;
    }
    
    public static bool GetBitDeposit(string metaName, int id)
    {
        SaveData data = LoadCachedSaveData();
        var entry = data.bitDeposits.FirstOrDefault(e => e == metaName);
        return entry != null;
    }
    
    public static bool GetLemonCollection(string metaName)
    {
        SaveData data = LoadCachedSaveData();
        var entry = data.lemonCollections.FirstOrDefault(e => e == metaName);
        return entry != null;
    }

    public static bool GetPersistentEventCompleted(string persistentEvent)
    {
        var data = LoadCachedSaveData();
        return data.persistentEvents.Contains(persistentEvent);
    }
    
    public static bool GetBell(string metaName)
    {
        var data = LoadCachedSaveData();
        return data.bells.Contains(metaName);
    }

    public static int GetBitCount()
    {
        var data = LoadCachedSaveData();
        return data.bitCount;
    }
    
    private static void WriteSaveData(SaveData saveData)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        SaveData data = saveData;
        data.scene = SceneManager.GetActiveScene().name;
        data.playTime += Singleton._timeSinceLastSave;
        Singleton._timeSinceLastSave = 0;
        Singleton._cachedSaveData = data;
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), json);
        Debug.Log("Saved file to: " + GetPath());
    }
    
    public static void WriteCachedSave()
    {
        WriteSaveData(Singleton._cachedSaveData);
    }

    public static SaveData LoadCachedSaveData()
    {
        if (Singleton._cachedSaveData == null)
        {
            string path = GetPath();
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
    
    
    public static SaveData LoadSaveDataFromDisk(int id)
    {
        var path = GetPath();
        if (!File.Exists(path))
        {
            return null;
        }

        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}

public static class MetaSaveSystem
{
    public static event Action<MetaSaveSystem.MetaSaveData> OnMetaSaveDataUpdated;
        
    [System.Serializable]
    public class MetaSaveData
    {

        public int saveId;
        public int cameraSensitivityModifier;
        public bool enableAmbientParticles;
        public bool enableFpsDisplay;
        public bool autoCamEnabled;
        
        public MetaSaveData(int saveId, int cameraSensitivityModifier, bool enableAmbientParticles, bool enableFpsDisplay, bool autoCamEnabled)
        {
            this.saveId = saveId;
            this.cameraSensitivityModifier = cameraSensitivityModifier;
            this.enableAmbientParticles = enableAmbientParticles;
            this.enableFpsDisplay = enableFpsDisplay;
            this.autoCamEnabled = autoCamEnabled;
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

    public static MetaSaveData WriteSaveId(int saveId)
    {
        var data = LoadMetaSaveData();
        data.saveId = saveId;
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetPath(), json);
        Debug.Log("Saved file to: " + GetPath());
        OnMetaSaveDataUpdated?.Invoke(data);
        return data;
    }
    
    public static MetaSaveData WriteMetaSaveData(int saveId, int cameraSensitivityModifier, bool enableAmbientParticles, bool enableFpsDisplay, bool autoCamEnabled)
    {
        string directory = GetDirectory();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        MetaSaveData data = new MetaSaveData(saveId, cameraSensitivityModifier, enableAmbientParticles, enableFpsDisplay, autoCamEnabled);
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
        return WriteMetaSaveData(0, 10, true, true, true);
    }
    
    
}