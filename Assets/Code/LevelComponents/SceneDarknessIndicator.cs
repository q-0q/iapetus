using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDarknessIndicator : MonoBehaviour
{

    public static event Action<float> OnPlayerLanternActivated;
    public float PlayerLanternRadiusMultiplier = 1.0f;
    public List<GameObject> disableWhenLanternActive;

    private void Awake()
    {
        foreach (var obj in disableWhenLanternActive)
        {
            obj.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (SaveSystem.GetAllItems().Contains("Lantern"))
        {
            OnPlayerLanternActivated?.Invoke(PlayerLanternRadiusMultiplier);
        }
        else
        {
            StartCoroutine(Coroutine());
        }
        
        // .ToArray() creates a temporary copy to iterate safely
        foreach (var customFogObserver in CustomFogManager.CustomFogObserverRegistry.ToArray())
        {
            if (!customFogObserver.isPlayer) continue;
            customFogObserver.enabled = false; // OnDisable() will safely modify the original list
        }



        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var obj in disableWhenLanternActive)
            {
                obj.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
