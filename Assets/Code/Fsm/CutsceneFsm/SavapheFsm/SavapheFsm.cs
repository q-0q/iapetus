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
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SavapheFsmState.NotCrossed;
        transform.Find("SavapheVirtualCamera").TryGetComponent(out _virtualCamera);
        
        _marker = transform.Find("Marker");
        _endPosition = transform.Find("EndPosition");
        _startPosition = transform.Find("StartPosition");
        _marker.position = _startPosition.position;
        
        _notCrossedDialogue = transform.Find("NotCrossedDialogue");
        _crossedDialogue = transform.Find("CrossedDialogue");
        _tutorialTrigger = transform.Find("TutorialTrigger");
        _tutorialTrigger.gameObject.SetActive(false);
        
        _virtualCamera.LookAt = _marker;

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Machine.IsInState(SavapheFsmState.NotCrossed))
        {
            _notCrossedDialogue.gameObject.SetActive(true);
            _crossedDialogue.gameObject.SetActive(false);
        }
        
        if (Machine.IsInState(SavapheFsmState.Crossing3))
        {
            _marker.position = Util.LerpWithArc(_startPosition.position, _endPosition.position, Util.SmoothLerp01(TimeInCurrentState() / 2.25f), 4f);
        }

        if (Machine.IsInState(SavapheFsmState.Crossed))
        {
            _notCrossedDialogue.gameObject.SetActive(false);
            _crossedDialogue.gameObject.SetActive(true);
        }
        
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        var saveData = SaveSystem.LoadSaveData(0);
        if (saveData.persistentEvents.Contains(CutscenePersistentEvent))
        {
            Machine.Jump(SavapheFsmState.Crossed);
            return;
        }
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
    }

    private void OnEnable()
    {
        SavapheCrossTrigger.SavapheCrossTriggerOnTriggerEnter += OnCrossTrigger;
        // _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        SavapheCrossTrigger.SavapheCrossTriggerOnTriggerEnter -= OnCrossTrigger;
        // _interactable.OnInteracted -= OnInteracted;
    }
}
