using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TestCutsceneFsm : CutsceneFsm
{
    public class TestCutsceneFsmState : CutsceneFsmState
    {
        public static int AlignCamera;
        public static int MoveCubeForward;
        public static int WaitForInput;
        public static int MoveCubeDown;
    }

    public class TestCutsceneFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {

    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = TestCutsceneFsmState.Inactive;
        TryGetComponent(out _interactable);
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
}
