using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Wasp;

public partial class YorbaFsm
{
    private Interactable _interactable;
    private DialogueController _dialogueController;
    private const string PersistentEvent = "c1-yorba-quest";
    private SkinnedMeshRenderer _fakeEyesRenderer;
    private Light _light;

    private const string ExpositionPersistentEvent = "YorbaExpositionHeard";
    
    private void OnDialogueProgressed(int textIndex)
    {
        
    }
    
}