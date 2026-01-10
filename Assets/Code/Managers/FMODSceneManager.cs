using System.Collections.Generic;
using UnityEngine;

public class FMODSceneManager : MonoBehaviour
{
    
    private static readonly Dictionary<string, string> MusicEventPath =
        new()
        {
            {"Cutscene", "event:/CH1_Music"}
        };
    
    private static readonly Dictionary<string, HashSet<string>> AmbienceEventPaths =
        new()
        {
            {
                "Cutscene", new HashSet<string>
                {
                    "event:/WindAmbience"
                }
            }
        };
    
    private static FMODSceneManager _singleton;
    public static FMODSceneManager Singleton
    {
        get
        {
            if (_singleton == null)
            {
                var go = new GameObject("FMODSceneManager");
                _singleton = go.AddComponent<FMODSceneManager>();
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

        _singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Foo()
    {
        print("foo");
    }
}


