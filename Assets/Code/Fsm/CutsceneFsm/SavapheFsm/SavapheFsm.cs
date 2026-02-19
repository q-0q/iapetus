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

public partial class SavapheFsm : CutsceneFsm
{
    public class SavapheFsmState : CutsceneFsmState
    {
        public static int NotCrossed;
        public static int Crossing1;
        public static int Crossing2;
        public static int Crossing3;
        public static int Crossed;
    }

    public class SavapheFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int PlayerCrossed;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        // TryGetComponent(out _interactable);
        
        _marker = transform.Find("Marker");
        _endPosition = transform.Find("EndPosition");
        _startPosition = transform.Find("StartPosition");
        _marker.position = _startPosition.position;
        _marker.rotation = _startPosition.rotation;
        
        _notCrossedDialogue = transform.Find("NotCrossedDialogue");
        _crossedDialogue = transform.Find("CrossedDialogue");
        _tutorialTrigger = transform.Find("TutorialTrigger");
        transform.Find("SavapheVirtualCameraA").TryGetComponent(out _virtualCameraA);
        transform.Find("SavapheVirtualCameraB").TryGetComponent(out _virtualCameraB);
        Animator = GetComponentInChildren<Animator>();

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SavapheFsmState.NotCrossed;
        
        

        _tutorialTrigger.gameObject.SetActive(false);
        
        _virtualCameraA.LookAt = _startPosition;
        _virtualCameraB.LookAt = _endPosition;
        
        _notCrossedDialogue.gameObject.SetActive(true);
        _crossedDialogue.gameObject.SetActive(false);

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        var playerNotCrossedDialogueDistance =
            Vector3.Distance(PlayerFsm.Singleton.transform.position, _notCrossedDialogue.position);
        
        Animator.SetFloat("Look", Mathf.InverseLerp(25f, 15f, playerNotCrossedDialogueDistance));
        
        if (Machine.IsInState(SavapheFsmState.NotCrossed))
        {
            
        }
        
        if (Machine.IsInState(SavapheFsmState.Crossing3))
        {
            
        }

        if (Machine.IsInState(SavapheFsmState.Crossed))
        {
            
        }
        
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        var saveData = SaveSystem.LoadSaveData(0);

        Machine.Jump(SaveSystem.GetPersistentEventCompleted(CutscenePersistentEvent)
            ? SavapheFsmState.Crossed
            : SavapheFsmState.NotCrossed);
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
    }

    private void OnEnable()
    {
        SavapheCrossTrigger.SavapheCrossTriggerOnTriggerEnter += OnCrossTrigger;
        _notCrossedDialogue.GetComponent<DialogueController>().OnCompleted += OnNotCrossedDialogueComplete;
        _notCrossedDialogue.GetComponent<Interactable>().OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        SavapheCrossTrigger.SavapheCrossTriggerOnTriggerEnter -= OnCrossTrigger;
        _notCrossedDialogue.GetComponent<DialogueController>().OnCompleted -= OnNotCrossedDialogueComplete;
        _notCrossedDialogue.GetComponent<Interactable>().OnInteracted -= OnInteracted;
    }
}
