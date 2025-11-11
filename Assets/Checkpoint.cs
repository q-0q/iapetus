using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private Interactable _interactable;
    private ParticleSystem _invokeParticles;

    private void Awake()
    {
        TryGetComponent(out _interactable);
        transform.Find("InvokeParticles").TryGetComponent(out _invokeParticles);
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }
    
    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    private void OnInteracted()
    {
        _invokeParticles.Play();
        PlayerFsm.Singleton.InvokeCheckpoint(PlayerFsm.Singleton.transform.position, PlayerFsm.Singleton.transform.rotation);
    }
}
