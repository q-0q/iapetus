using System;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class CrabGuardFsm
{
    private Interactable _interactable;
    private DialogueController _dialogueController;
    private const string PersistentEvent = "c1-snail-quest";
    private bool _triggerActive;
    private float _turnAmount;
    
    private void OnDialogueProgressed(int textIndex)
    {

    }
    
    private void OnTriggerProxyStay(Collider obj)
    {
        _triggerActive = true;
        var pos = transform.InverseTransformPoint(obj.transform.position);
        if (pos.z < 0) _turnAmount = Mathf.InverseLerp(-8f, 8f, pos.x);
        else _turnAmount = pos.x > 0 ? 1f : 0f;
    }

    private void OnTriggerProxyExit(Collider obj)
    {
        _triggerActive = false;
    }
}