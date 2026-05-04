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

public class CultistMinorFsm : CultistFsm
{

    
    public class CultistMinorFsmState : CultistFsmState
    {

    }

    public class CultistMinorFsmTrigger : CultistFsmTrigger
    {
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        
        circletRenderer.enabled = false;
        robeDetailRenderer.enabled = false;

    }

    protected override void OnStart()
    {
        base.OnStart();
        
        //
        
    }
    
    public override void SetupMachine()
    {
        base.SetupMachine();



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
