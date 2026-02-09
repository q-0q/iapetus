using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wasp;

public partial class SavapheFsm : CutsceneFsm
{
    public class SavapheFsmState : CutsceneFsmState
    {
        public static int NotCrossed;
        public static int Crossing;
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
        _endPosition = transform.Find("EndPosition");
        _endPosition.SetParent(null);

        var cameraFollow = FindObjectOfType<CameraFollow>().transform;
        _virtualCamera.Follow = cameraFollow;
        _virtualCamera.LookAt = cameraFollow;

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Machine.IsInState(SavapheFsmState.NotCrossed))
        {

        }
        
        if (Machine.IsInState(SavapheFsmState.Crossing))
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
        // _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        // _interactable.OnInteracted -= OnInteracted;
    }
}
