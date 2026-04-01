using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class SavapheFsm
{
    // private Interactable _interactable;
    private CinemachineVirtualCamera _virtualCameraA;
    private CinemachineVirtualCamera _virtualCameraB;
    private const string CutscenePersistentEvent = "SavapheCrossed";

    private Transform _endPosition;
    private Transform _startPosition;
    private Transform _marker;
    private Transform _crossTrigger;

    private Transform _notCrossedDialogue;
    private Transform _crossedDialogue;

    private void OnCrossTrigger()
    {
        Machine.Fire(SavapheFsmTrigger.PlayerCrossed);
    }

    private void OnNotCrossedDialogueComplete()
    {
        // ReplaceAnimatorTrigger("NotCrossedDialogueComplete");
    }
}