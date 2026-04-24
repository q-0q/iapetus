using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class CultTrialFsm : Fsm
{
    public class CultTrialFsmState : FsmState
    {
        public static int LockedIdle;
        public static int LockedDialogue;
        public static int UnlockedIdle;
        public static int FirstTimeUseDialogue1;
        public static int FirstTimeUseDialogue2;
        public static int FirstTimeUseDialogue3;

        
        public static int ApplyingCurse;
    }

    public class CultTrialFsmTrigger : FsmTrigger
    {
        public static int OnInteracted;
        public static int OnUnlock;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        
        UpdateKeyframes();
        _dialogueNoItem = CultTrialManager.Singleton.dialogueNoItem;
        _dialogueItem = CultTrialManager.Singleton.dialogueItem;
        _dialogueFirstTimeUse1 = CultTrialManager.Singleton.dialogueFirstTimeUse1;
        _dialogueFirstTimeUse2 = CultTrialManager.Singleton.dialogueFirstTimeUse2;
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CultTrialFsmState.LockedIdle;
        _interactable.SetEnabled(true);

        _activeFogController = transform.Find("StartingLine").Find("ActiveFogController")
            .GetComponent<CustomFogController>();

        _startingLineBaseMaterial = transform.Find("StartingLine").Find("Base").GetComponent<Renderer>().material;
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        if (SaveSystem.GetPersistentEventCompleted(metaName + "-unlocked"))
        {
            AssumeActivation();
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(CultTrialFsmState.ApplyingCurse))
        {
            CultTrialManager.Singleton.SetCurseEffects(TimeInCurrentState());
        }

        
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        _dialogueItem.OnCompleted += OnDialogueCompleted;
        _dialogueNoItem.OnCompleted += OnDialogueCompleted;
        _dialogueFirstTimeUse1.OnCompleted += OnDialogueCompleted;
        _dialogueFirstTimeUse2.OnCompleted += OnDialogueCompleted;

    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueItem.OnCompleted -= OnDialogueCompleted;
        _dialogueNoItem.OnCompleted -= OnDialogueCompleted;
        _dialogueFirstTimeUse1.OnCompleted -= OnDialogueCompleted;
        _dialogueFirstTimeUse2.OnCompleted -= OnDialogueCompleted;
    }

    private void OnDrawGizmos()
    {
        AlignStartingPosition();
    }
}
