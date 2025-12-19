using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

public partial class TestCutsceneFsm : CutsceneFsm
{
    public class TestCutsceneFsmState : CutsceneFsmState
    {
        public static int AlignCamera;
        public static int ShowText;
        public static int TextFade;
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
        public static int TextComplete;
        public static int PlayerInputJump;
        public static int PlayerInJumpState;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        // TryGetComponent(out _interactable);
    }

    protected override void OnStart()
    {
        base.OnStart();
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        InitState = TestCutsceneFsmState.Inactive;
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transform.Find("IntroCutsceneCanvas").TryGetComponent(out _mainCanvasGroup);
        transform.Find("IntroCutsceneCanvas").Find("TextCanvasGroup").TryGetComponent(out _textCanvasGroup);
        _playerTransformOnStart = transform.Find("PlayerTransformOnStart");
        _textTmp = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Machine.IsInState(TestCutsceneFsmState.ShowText))
        {
            _textClock += Time.deltaTime;
            var text = texts[_currentTextId];
            var textCharId = Math.Min((int)(_textClock / 0.04f), text.Length);
            var newTextStr = text.Substring(0, textCharId) + "<alpha=#00>" + text.Substring(textCharId);
            _textTmp.text = newTextStr;

            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                if (textCharId < text.Length) _textClock = 100f;
                else if (_currentTextId < texts.Count - 1)
                {
                    print("a");
                    _currentTextId++;
                    _textClock = 0f;
                }
                else
                {
                    print("b");
                    Machine.Fire(TestCutsceneFsmTrigger.TextComplete);
                }
            }
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.TextFade))
        {
            _textCanvasGroup.alpha = Mathf.Lerp(1f, 0.0f, Mathf.InverseLerp(0f, 2f, TimeInCurrentState()));
        }

        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeForward))
        {
            gondola.transform.position += gondola.forward * (Time.deltaTime * 10f);
            _mainCanvasGroup.alpha = Mathf.Lerp(1f, 0.0f, Mathf.InverseLerp(0f, 5f, TimeInCurrentState()));
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown1))
        {
            gondola.transform.position += gondola.up * (TimeInCurrentState() * TimeInCurrentState() * -100f * Time.deltaTime);
            Time.timeScale = Mathf.Lerp(1f, 0.01f, Mathf.InverseLerp(0.55f, 0.65f, TimeInCurrentState()));
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown2))
        {
            gondola.transform.position += gondola.up * (-60f * Time.deltaTime);
        }

        if (Machine.IsInState(TestCutsceneFsmState.Shake2))
        {
            
        }
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        Machine.Fire(CutsceneFsmTrigger.StartCutscene);
    }

    private void OnEnable()
    {
        // _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        // _interactable.OnInteracted -= OnInteracted;
    }
}
