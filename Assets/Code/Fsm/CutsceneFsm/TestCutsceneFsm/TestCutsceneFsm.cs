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
        public static int Shake1;
        public static int MoveCubeDown1;
        public static int WaitForInput;
        public static int WaitForJumpsquat;
        public static int MoveCubeDown2;
        public static int Shake2;
    }

    public class TestCutsceneFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int PlayerInputJump;
        public static int PlayerInJumpState;
        
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        TryGetComponent(out _interactable);
    }

    protected override void OnStart()
    {
        base.OnStart();
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        InitState = TestCutsceneFsmState.Inactive;
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeForward))
        {
            cube.transform.position += cube.forward * (Time.deltaTime * 10f);
            _canvasGroup.alpha = Mathf.Lerp(1f, 0.0f, Mathf.InverseLerp(0f, 3f, TimeInCurrentState()));
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown1))
        {
            cube.transform.position += cube.up * (TimeInCurrentState() * TimeInCurrentState() * -100f * Time.deltaTime);
            Time.timeScale = Mathf.Lerp(1f, 0.01f, Mathf.InverseLerp(0.55f, 0.65f, TimeInCurrentState()));
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown2))
        {
            cube.transform.position += cube.up * (-60f * Time.deltaTime);
        }

        if (Machine.IsInState(TestCutsceneFsmState.Shake2))
        {
            
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
