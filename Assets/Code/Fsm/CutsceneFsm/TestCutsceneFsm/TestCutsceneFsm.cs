using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
        TryGetComponent(out _interactable);
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = TestCutsceneFsmState.Inactive;
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeForward))
        {
            cube.transform.position += cube.forward * (Time.deltaTime * 10f);
        }
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
