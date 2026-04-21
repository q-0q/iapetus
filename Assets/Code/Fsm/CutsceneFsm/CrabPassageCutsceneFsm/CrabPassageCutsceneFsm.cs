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

public partial class CrabPassageCutsceneFsm : CutsceneFsm
{
    public class CrabPassageCutsceneFsmState : CutsceneFsmState
    {
        public static int Warning1;
        public static int Warning2;
        public static int Channel;
    }

    public class CrabPassageCutsceneFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
        public static int Trigger1;
        public static int Trigger2;
        public static int Trigger3;
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _warningCamera = transform.Find("CrabPassageWarningCamera").GetComponent<CinemachineVirtualCamera>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CutsceneFsmState.Inactive;
        
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

    private void OnEnable()
    {
        warning1.OnCompleted += OnDialogueCompleted;
        warning1.OnProgressed += OnDialogueProgressed;
        warning2.OnCompleted += OnDialogueCompleted;
        warning2.OnProgressed += OnDialogueProgressed;
        CutsceneTrigger1.OnTriggerProxyStay += OnTrigger1;
        CutsceneTrigger2.OnTriggerProxyStay += OnTrigger2;
        CutsceneTrigger3.OnTriggerProxyStay += OnTrigger3;
    }

    private void OnDisable()
    {
        warning1.OnCompleted -= OnDialogueCompleted;
        warning1.OnProgressed -= OnDialogueProgressed;
        warning2.OnCompleted -= OnDialogueCompleted;
        warning2.OnProgressed -= OnDialogueProgressed;
    }
}
