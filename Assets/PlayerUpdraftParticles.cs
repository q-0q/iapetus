using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdraftParticles : MonoBehaviour
{

    private ParticleSystem _particleSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _particleSystem);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Updraft))
        {
            _particleSystem.Stop();
        }
    }

    private void OnPlayerEnterUpdraft()
    {
        print(transform.parent.name);
        _particleSystem.Play();
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerEnterUpdraft += OnPlayerEnterUpdraft;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerEnterUpdraft -= OnPlayerEnterUpdraft;
    }
}
