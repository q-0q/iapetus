using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadInteractable : MonoBehaviour
{

    public string DestinationId;
    public string SceneName;
    private Interactable _interactable;

    private void Awake()
    {
        TryGetComponent(out _interactable);
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
        SaveSystem.WritePlayerInGamePosition(Vector3.zero, DestinationId, 0);
        SceneLoader.Singleton.LoadScene(SceneName);
    }
}
