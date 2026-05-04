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

public class CultistCosmeticFsm : CultistFsm
{
    private const string ItemGivenPersistentEventSuffix = "CultistCosmeticGiven";
    private KeyItem _keyItem;
    public string dyeName = "Yellow";
    public static event Action<string> OnIncenseGiven;
    
    public class CultistCosmeticFsmState : CultistFsmState
    {

    }

    public class CultistCosmeticFsmTrigger : CultistFsmTrigger
    {
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        _keyItem = GetComponentInChildren<KeyItem>();
        _keyItem.gameObject.SetActive(false);
        DialogueController.currentDialogueIndex = SaveSystem.GetPersistentEventCompleted(GetItemGivenPersistentEvent()) ? 1 : 0;
        
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
                DialogueController.currentDialogueIndex = 1;
                Interactable.SetEnabled(false);

                StartCoroutine(CampId == 0 ? IncenseBurnerCoroutine() : IncenseCoroutine());

                IEnumerator IncenseBurnerCoroutine()
                {
                    _keyItem.gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                    _keyItem.OnInteracted();
                    // SaveSystem.AddDye(dyeName);
                    SaveSystem.WritePersistentEvent(GetItemGivenPersistentEvent());

                }
                
                IEnumerator IncenseCoroutine()
                {
                    yield return new WaitForSeconds(0.5f);
                    OnIncenseGiven?.Invoke(dyeName + " dye");
                    SaveSystem.WritePersistentEvent(GetItemGivenPersistentEvent());
                    // SaveSystem.AddDye(dyeName);
                }
            })
            .OnExit(_ =>
            {
                Interactable.SetEnabled(true);
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
        
        // if (!SaveSystem.GetPersistentEventCompleted(GetItemGivenPersistentEvent())) Machine.Jump(CultistFsmState.Give);
    }

    private string GetItemGivenPersistentEvent()
    {
        return ItemGivenPersistentEventSuffix + CampId;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        CultistIncenseFsm.OnItemGiven += OnDance;
    }

    private void OnDance()
    {
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CultistIncenseFsm.OnItemGiven -= OnDance;
    }
}
