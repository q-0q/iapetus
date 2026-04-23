using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CultTrialFsm : Fsm
{
    public class CultTrialFsmState : FsmState
    {
        public static int Disabled;
    }

    public class CultTrialFsmTrigger : FsmTrigger
    {
        public static int Toggle;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        
        UpdateKeyframes();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CultTrialFsmState.Disabled;
        _interactable.SetEnabled(true);

        transform.Find("DialogueNoItem").TryGetComponent(out _dialogueNoItem);
        transform.Find("DialogueItem").TryGetComponent(out _dialogueItem);
        transform.Find("DialogueFirstTimeUse").TryGetComponent(out _dialogueFirstTimeUse);

        _startingLineBaseMaterial = transform.Find("StartingLine").Find("Base").GetComponent<Renderer>().material;

        if (SaveSystem.GetPersistentEventCompleted(metaName + "-unlocked"))
        {
            AssumeActivation();
        }
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;

    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    private void OnDrawGizmos()
    {
        AlignStartingPosition();
    }
}
