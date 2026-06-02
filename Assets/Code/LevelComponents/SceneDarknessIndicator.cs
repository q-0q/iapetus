using System;
using UnityEngine;

public class SceneDarknessIndicator : MonoBehaviour
{

    public static event Action OnPlayerLanternActivated;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var customFogObserver in CustomFogManager.CustomFogObserverRegistry)
        {
            if (!customFogObserver.isPlayer) continue;
            if (SaveSystem.GetAllItems().Contains("Lantern"))
            {
                OnPlayerLanternActivated?.Invoke();
            }
            else customFogObserver.enabled = false;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
