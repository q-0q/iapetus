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

public partial class SnailHunterFsm : CutsceneFsm
{
    public class SnailHunterFsmState : CutsceneFsmState
    {
        public static int Idle;
        public static int SpeakingDefault;
        public static int SpeakingQuestReady;
        public static int QuestChannel;
    }

    public class SnailHunterFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        _dialogueController = GetComponentInChildren<DialogueController>();
        // Animator = GetComponentInChildren<Animator>();

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SnailHunterFsmState.Idle;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        
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
