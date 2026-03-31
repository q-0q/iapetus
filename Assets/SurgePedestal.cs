using System;
using System.Collections;
using System.Collections.Generic;
using Code.TriggerParams;
using UnityEngine;

public class SurgePedestal : MonoBehaviour
{
    private Interactable _interactable;
    private Material _material;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _material = GetComponent<Renderer>().material;
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
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.SurgePedestalInteracted, new MaterialParam() { Material = _material });
    }
}
