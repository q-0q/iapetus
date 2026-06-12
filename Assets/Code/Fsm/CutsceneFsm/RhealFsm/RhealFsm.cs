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

public partial class RhealFsm : CutsceneFsm
{
    public class RhealFsmState : CutsceneFsmState
    {
        public static int Idle;
        public static int Speaking;
    }

    public class RhealFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
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

        if (SaveSystem.GetTrick("Tinsica")) _dialogueController.currentDialogueIndex = 3;

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = RhealFsmState.Idle;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        var turnAmount = 0.5f;
        var pos = transform.InverseTransformPoint(PlayerFsm.Singleton.transform.position);
        turnAmount = Mathf.Lerp(0.2f, 0.8f, Mathf.InverseLerp(-8f, 8f, pos.x));
        Animator.SetFloat("Turn", Mathf.Lerp(Animator.GetFloat("Turn"), turnAmount, Time.deltaTime * 5f));

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
