using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wasp;
using Util = Code.Misc.Util;

public partial class SnailMerchantFsm : CutsceneFsm
{
    public class SnailMerchantFsmState : CutsceneFsmState
    {
        public static int Idle;
        public static int SpeakingDefault;
        public static int SpeakingQuestReady;
        public static int QuestChannel;
        public static int SpeakingQuestComplete;
    }

    public class SnailMerchantFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        _dialogueController = GetComponentInChildren<DialogueController>();
        _attenuator = GetComponentInChildren<MusicDistanceAttenuator>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        Animator = GetComponentInChildren<Animator>();

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SnailMerchantFsmState.Idle;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(SnailMerchantFsmState.QuestChannel))
        {
            var attenuationDistance = Mathf.Lerp(0, 10f, Mathf.InverseLerp(0, 7f, TimeInCurrentState()));
            _attenuator.minDistance = attenuationDistance;
            _attenuator.maxDistance = attenuationDistance * 10f;

            _canvasGroup.alpha = Mathf.InverseLerp(3f, 8f, TimeInCurrentState());
            _haloRenderer.material.SetFloat("_Weight", Mathf.InverseLerp(0.5f, 1.5f, TimeInCurrentState()));
        }
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        var saveData = SaveSystem.LoadSaveData(0);
        
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        _dialogueController.OnCompleted += OnDialogueCompleted;
        _dialogueController.OnProgressed += OnDialogueProgressed;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueController.OnCompleted -= OnDialogueCompleted;
        _dialogueController.OnProgressed -= OnDialogueProgressed;
    }
}
