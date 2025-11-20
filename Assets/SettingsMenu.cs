using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{

    public static event Action OnSettingsMenuClosed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
        OnSettingsMenuClosed?.Invoke();
    }
    
    
}
