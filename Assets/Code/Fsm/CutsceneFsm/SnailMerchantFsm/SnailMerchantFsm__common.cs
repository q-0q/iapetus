using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class SnailMerchantFsm
{
    private Interactable _interactable;
    private DialogueController _dialogueController;
    private const int QuestBitRequirement = 500;


    private bool IsQuestCompleted()
    {
        return SaveSystem.GetBitCount() > QuestBitRequirement;
    }
}