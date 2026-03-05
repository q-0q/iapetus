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
    private const string PersistentEvent = "c1-snail-quest";
    private MusicDistanceAttenuator _attenuator;
    private CanvasGroup _canvasGroup;
    public Renderer _haloRenderer;


    private bool IsBitRequirementMet()
    {
        return SaveSystem.GetBitCount() > QuestBitRequirement;
    }

    private void OnDialogueProgressed(int textIndex)
    {
        if (textIndex != 2) return;
        if (!Machine.IsInState(SnailMerchantFsmState.SpeakingQuestReady)) return;
        if (SaveSystem.GetPersistentEventCompleted(PersistentEvent)) return;
        SaveSystem.WritePersistentEvent(PersistentEvent, 0);
        BitSystem.Singleton.RemoveBits(500);
    }
}