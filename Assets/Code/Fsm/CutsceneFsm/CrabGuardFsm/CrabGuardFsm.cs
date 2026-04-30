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

public partial class CrabGuardFsm : CutsceneFsm
{
    public class CrabGuardFsmState : CutsceneFsmState
    {
        public static int IdleDefault;
        public static int SpeakingDefault;
        public static int IdleQuestComplete;
        public static int SpeakingQuestComplete;
        public static int Channel;
    }

    public class CrabGuardFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
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
        _questDestination = transform.Find("QuestDestination");

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CrabGuardFsmState.IdleDefault;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        var pos = transform.InverseTransformPoint(PlayerFsm.Singleton.transform.position);
        if (pos.z < 0) _turnAmount = Mathf.InverseLerp(-8f, 8f, pos.x);
        else _turnAmount = pos.x > 0 ? 1f : 0f;
        
        if (Machine.IsInState(CrabGuardFsmState.IdleDefault) || Machine.IsInState(CrabGuardFsmState.SpeakingQuestComplete))
        {
            var newTurnAmount = _turnAmount;
            Animator.SetFloat("Turn", Mathf.Lerp(Animator.GetFloat("Turn"), newTurnAmount, Time.deltaTime * 5f));
        }
        else
        {
            Animator.SetFloat("Turn", Mathf.Lerp(Animator.GetFloat("Turn"), 0.5f, Time.deltaTime * 5f));
        }
        
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();

        if (SaveSystem.GetPersistentEventCompleted(YorbaFsm.PersistentEvent))
        {
            transform.position = _questDestination.position;
            transform.rotation = _questDestination.rotation;
            Machine.Jump(CrabGuardFsmState.IdleQuestComplete);
        }

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
        CrabPassageCutsceneFsm.OnChannel += OnChannel;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueController.OnCompleted -= OnDialogueCompleted;
        _dialogueController.OnProgressed -= OnDialogueProgressed;
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyStay -= OnTriggerProxyStay;
        GetComponentInChildren<TriggerProxy>().OnTriggerProxyExit -= OnTriggerProxyExit;
        CrabPassageCutsceneFsm.OnChannel -= OnChannel;
    }
}
