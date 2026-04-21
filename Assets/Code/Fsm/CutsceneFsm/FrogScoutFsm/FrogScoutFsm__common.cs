using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class FrogScoutFsm
{
    private Interactable _interactable;
    private DialogueController _dialogueController;
    private const string PersistentEvent = "c1-snail-quest";

    
    private void OnDialogueProgressed(int textIndex)
    {

    }
}