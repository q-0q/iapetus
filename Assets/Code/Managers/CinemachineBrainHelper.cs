using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineBrainHelper : MonoBehaviour
{
    private CinemachineBrain _brain;

    void Start()
    {
        TryGetComponent(out _brain);
    }
    
    void Update()
    {
        if (!GameMenu.Singleton.IsMenuOpen()) _brain.ManualUpdate();
    }
}
