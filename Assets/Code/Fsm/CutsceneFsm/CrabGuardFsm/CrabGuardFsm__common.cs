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

    }

    private void OnTriggerProxyExit(Collider obj)
    {
        _triggerActive = false;
    }
}