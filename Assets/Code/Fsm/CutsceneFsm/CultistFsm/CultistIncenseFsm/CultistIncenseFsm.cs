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

public class CultistIncenseFsm : CultistFsm
{
    private const string ItemGivenPersistentEventSuffix = "CultistIncenseGiven";
    public class CultistIncenseFsmState : CultistFsmState
    {

    }

    public class CultistIncenseFsmTrigger : CultistFsmTrigger
    {
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        DialogueController.currentDialogueIndex =
            SaveSystem.GetPersistentEventCompleted(GetItemGivenPersistentEvent()) ? 1 : 0;

        circletRenderer.enabled = false;

    }

    protected override void OnStart()
    {
        base.OnStart();
        
        //
        
    }
    
    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CultistFsmState.Give)
            .OnEntry(_ =>
            {
                SaveSystem.WritePersistentEvent(GetItemGivenPersistentEvent());
                DialogueController.currentDialogueIndex = 1;
            });

    }
    
    public override void SetupStateMaps()
    {
        base.SetupStateMaps();

        

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
    }
    
    protected override void OnDialogueCompleted()
    {
        base.OnDialogueCompleted();
        
        if (!SaveSystem.GetPersistentEventCompleted(GetItemGivenPersistentEvent())) Machine.Jump(CultistFsmState.Give);
    }

    private string GetItemGivenPersistentEvent()
    {
        return ItemGivenPersistentEventSuffix + CampId;
    }
}
