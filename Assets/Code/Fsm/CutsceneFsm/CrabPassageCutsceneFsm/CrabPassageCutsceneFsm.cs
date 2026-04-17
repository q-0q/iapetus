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

public partial class CrabPassageCutsceneFsm : CutsceneFsm
{
    public class CrabPassageCutsceneFsmState : CutsceneFsmState
    {
        public static int IdleDefault;
        public static int SpeakingDefault;
        public static int IdleQuestComplete;
        public static int SpeakingQuestComplete;
        public static int SpeakingForced;
    }

    public class CrabPassageCutsceneFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        _dialogueController = GetComponentInChildren<DialogueController>();
        Animator = GetComponentInChildren<Animator>();

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CrabPassageCutsceneFsmState.IdleDefault;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(CrabPassageCutsceneFsmState.IdleDefault))
        {
            var newTurnAmount = _triggerActive ? _turnAmount : 0.5f;
            Animator.SetFloat("Turn", Mathf.Lerp(Animator.GetFloat("Turn"), newTurnAmount, Time.deltaTime * 5f));
        }
        
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
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyStay += OnTriggerProxyStay;
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyExit += OnTriggerProxyExit;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueController.OnCompleted -= OnDialogueCompleted;
        _dialogueController.OnProgressed -= OnDialogueProgressed;
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyStay -= OnTriggerProxyStay;
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyExit -= OnTriggerProxyExit;
    }
}
