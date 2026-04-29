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

        
        public static int ApplyingCurse;
        
        public static int TrialActive;
        public static int Complete;
        public static int FirstTimeUseDialogue4;
        public static int RemovingCurse;
    }

    public class CultTrialFsmTrigger : FsmTrigger
    {
        public static int OnInteracted;
        public static int OnUnlock;
        public static int OnDialogueCompleted;

        public static int PlayerLeftStartingLine;
        public static int PlayerTrialDeath;

        public static int FinalKeyframeTriggered;
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
        _dialogueFirstTimeUse3 = CultTrialManager.Singleton.dialogueFirstTimeUse3;
        _dialogueFirstTimeUse4 = CultTrialManager.Singleton.dialogueFirstTimeUse4;
        
        UpdateInteractable();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CultTrialFsmState.LockedIdle;
        _interactable.SetEnabled(true);



        _startingLine = transform.Find("StartingLine");
        _startingLineBaseMaterial = _startingLine.Find("Base").GetComponent<Renderer>().material;
        _gemMaterial = _startingLine.Find("Gem").GetComponent<Renderer>().material;
        
        if (SaveSystem.GetPersistentEventCompleted(metaName+"-complete")) _gemMaterial.SetFloat("_Weight_1", 1);
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
        
        if (Machine.IsInState(CultTrialFsmState.RemovingCurse))
        {
            CultTrialManager.Singleton.SetCurseEffects(1.5f - TimeInCurrentState() * 2.5f);
        }
        
        if (Machine.IsInState(CultTrialFsmState.Complete))
        {
            Time.timeScale = Mathf.Lerp(1f, 0.15f, Mathf.InverseLerp(0, 0.2f, TimeInCurrentState()));
        }

        
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        _dialogueItem.OnCompleted += OnDialogueCompleted;
        _dialogueNoItem.OnCompleted += OnDialogueCompleted;
        _dialogueFirstTimeUse1.OnCompleted += OnDialogueCompleted;
        _dialogueFirstTimeUse2.OnCompleted += OnDialogueCompleted;
        _dialogueFirstTimeUse4.OnCompleted += OnDialogueCompleted;
        PlayerFsm.OnPlayerCultTrialDeath += OnPlayerCultTrialDeath;

    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueItem.OnCompleted -= OnDialogueCompleted;
        _dialogueNoItem.OnCompleted -= OnDialogueCompleted;
        _dialogueFirstTimeUse1.OnCompleted -= OnDialogueCompleted;
        _dialogueFirstTimeUse2.OnCompleted -= OnDialogueCompleted;
        _dialogueFirstTimeUse4.OnCompleted -= OnDialogueCompleted;
        PlayerFsm.OnPlayerCultTrialDeath -= OnPlayerCultTrialDeath;
    }

    private void OnDrawGizmos()
    {
        AlignStartingPosition();
    }
}
