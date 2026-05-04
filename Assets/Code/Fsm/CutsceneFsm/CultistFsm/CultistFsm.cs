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

public abstract partial class CultistFsm : CutsceneFsm
{
    public class CultistFsmState : CutsceneFsmState
    {
        public static int Idle;
        public static int Give;
        public static int Dancing;
    }

    public class CultistFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        Interactable = GetComponentInChildren<Interactable>();
        DialogueController = GetComponentInChildren<DialogueController>();
        Animator = GetComponentInChildren<Animator>();
        CampId = transform.parent.parent.GetComponent<CultistCamp>().campId;

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CultistFsmState.Idle;
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        var pos = transform.InverseTransformPoint(PlayerFsm.Singleton.transform.position);
        if (pos.z > -3f) _turnAmount = Mathf.InverseLerp(-8f, 8f, pos.x);
        else _turnAmount = 0.5f;
        
        if (Machine.IsInState(CultistFsmState.Idle))
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
        
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
    }

    protected virtual void OnEnable()
    {
        Interactable.OnInteracted += OnInteracted;
        DialogueController.OnCompleted += OnDialogueCompleted;
        DialogueController.OnProgressed += OnDialogueProgressed;
    }

    protected virtual void OnDisable()
    {
        Interactable.OnInteracted -= OnInteracted;
        DialogueController.OnCompleted -= OnDialogueCompleted;
        DialogueController.OnProgressed -= OnDialogueProgressed;
    }
}
