using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Misc;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using Wasp;

public partial class TestCutsceneFsm : CutsceneFsm
{
    public class TestCutsceneFsmState : CutsceneFsmState
    {
        public static int AlignCamera;
        public static int ShowText;
        public static int TextFade;
        public static int PlayerControl;
        public static int InteractableReady1;
        public static int InteractableChannel1;
        public static int InteractableReady2;
        public static int InteractableChannel2;
        public static int InteractableReady3;
        public static int Channel;
        public static int ChannelEnd;
        public static int CanvasFade;
        public static int Shake1;
        public static int MoveCubeDown1;
        public static int WaitForInput;
        public static int WaitForJumpsquat;
        public static int MoveCubeDown2;
        public static int Shake2;
        public static int FinalCamera;
        public static int MoveForward;

    }

    public class TestCutsceneFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int TextComplete;
        public static int PlayerInputJump;
        public static int PlayerInJumpState;
        public static int OnInteracted;
        public static int OnTriggersCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        _interactableParticles = transform.Find("InteractableParticles").GetComponent<ParticleSystem>();
        _particlesHalo = _interactableParticles.transform.Find("Halo");
        _particlesHaloMaterial = _particlesHalo.GetComponent<Renderer>().material;
        _particlesHalo.SetParent(null);
        innerCube.TryGetComponent(out Animator);
        _backgroundParent = transform.Find("BackgroundParent");
        _backgroundParent.SetParent(null);

        // TryGetComponent(out _interactable);
    }

    protected override void OnStart()
    {
        base.OnStart();
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        InitState = TestCutsceneFsmState.Inactive;
        transform.Find("TestCutsceneVirtualCamera").TryGetComponent(out _virtualCamera);
        transform.Find("TestCutsceneFinalVirtualCamera").TryGetComponent(out _finalVirtualCamera);
        transform.Find("TestCutsceneChannelVirtualCamera").TryGetComponent(out _channelCamera);
        _channelDolly = _channelCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        
        
        transform.Find("IntroCutsceneCanvas").TryGetComponent(out _mainCanvasGroup);
        transform.Find("IntroCutsceneCanvas").Find("TextCanvasGroup").TryGetComponent(out _textCanvasGroup);
        _playerTransformOnStart = transform.Find("PlayerTransformOnStart");
        _textTmp = GetComponentInChildren<TextMeshProUGUI>();
        transform.Find("ImpactParticles").TryGetComponent(out _impactParticles);
        _stateGondolaStartingPosition = transform.position;

        _interactablePosA = transform.Find("InteractablePosA").localPosition;
        _interactablePosB = transform.Find("InteractablePosB").localPosition;
        _interactableParticlesPosA = transform.Find("InteractableParticlesPosA").localPosition;
        _interactableParticlesPosB = transform.Find("InteractableParticlesPosB").localPosition;

        var cameraFollow = FindObjectOfType<CameraFollow>().transform;
        _virtualCamera.Follow = cameraFollow;
        _virtualCamera.LookAt = cameraFollow;
        _finalVirtualCamera.Follow = cameraFollow;
        _finalVirtualCamera.LookAt = cameraFollow;
        
        _initialFogEndDistance = RenderSettings.fogEndDistance;
        _initialFogStartDistance = RenderSettings.fogStartDistance;

        _channelSpline = transform.Find("ChannelSpline").GetComponent<SplineContainer>();
        _minorSpline = transform.Find("MinorSpline").GetComponent<SplineContainer>();
        _currentNeededTriggerId = "a";


    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        _particlesHaloMaterial.SetFloat("_Weight", Mathf.Lerp(_particlesHaloMaterial.GetFloat("_Weight"), _interactable.isEnabled ? 1f : 0f, Time.deltaTime * 4f));
        if (_interactable.isEnabled) _particlesHalo.position = _interactableParticles.transform.position;
        
        if (Machine.IsInState(TestCutsceneFsmState.ShowText))
        {
            _textClock += Time.deltaTime;
            var text = texts[_currentTextId];
            var textCharId = Math.Min((int)(_textClock / 0.04f), text.Length);
            var newTextStr = text.Substring(0, textCharId) + "<alpha=#00>" + text.Substring(textCharId);
            _textTmp.text = newTextStr;

            if ((int)(_textClock / 0.04f) >= text.Length + 25)
            {
                textAdvanceImage.enabled = true;
                textAdvanceImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Interact");
            }

            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                if (textCharId < text.Length) _textClock = 100f;
                else if (_currentTextId < texts.Count - 1)
                {
                    textAdvanceImage.enabled = false;
                    _currentTextId++;
                    _textClock = 0f;
                }
                else
                {
                    textAdvanceImage.enabled = false;
                    Machine.Fire(TestCutsceneFsmTrigger.TextComplete);
                }
            }
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.TextFade))
        {
            _textCanvasGroup.alpha = Mathf.Lerp(1f, 0.0f, Mathf.InverseLerp(0f, 2f, TimeInCurrentState()));
            if (_playerInput.actions["Interact"].WasPressedThisFrame() && TimeInCurrentState() > 0.1f)
            {
                Machine.Fire(CutsceneFsmTrigger.Skip);
            }
        }

        if (Machine.IsInState(TestCutsceneFsmState.CanvasFade))
        {
            // var position = _endPosition.position;
            // gondola.transform.position = Vector3.Lerp(_stateGondolaStartingPosition,
            //     new Vector3(position.x, _stateGondolaStartingPosition.y,
            //         position.z), Mathf.InverseLerp(0, _moveCubeForwardDuration, TimeInCurrentState()));
            
            _mainCanvasGroup.alpha = Mathf.Lerp(1f, 0.0f, Mathf.InverseLerp(0f, CanvasFadeDuration, TimeInCurrentState()));

            // if (_playerInput.actions["Interact"].WasPressedThisFrame())
            // {
            //     Machine.Fire(CutsceneFsmTrigger.Skip);
            // }

                
            // if (TimeInCurrentState() > 6f && !_moveCubeForwardShake1)
            // {
            //     _moveCubeForwardShake1 = true;
            //     FMODUnity.RuntimeManager.PlayOneShotAttached(gondolaMinorBangEventReference, gondola.gameObject);
            //     innerCube.DOShakePosition(6.75f, 0.25f);
            //     innerCube.DOShakeRotation(6.75f, 0.5f);
            // }
            
            // if (TimeInCurrentState() > 12f && !_moveCubeForwardShake2)
            // {
            //     _moveCubeForwardShake2 = true;
            //     FMODUnity.RuntimeManager.PlayOneShotAttached(gondolaMinorBangEventReference, gondola.gameObject);
            //     innerCube.DOShakePosition(6.75f, 0.25f);
            //     innerCube.DOShakeRotation(6.75f, 0.5f);
            // }
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveForward))
        {
            gondola.transform.position += Vector3.forward * (Time.deltaTime * 8f);
            if (!_waitingToSpawnBackgroundElement && !Machine.IsInState(TestCutsceneFsmState.Channel))
            {
                StartCoroutine(SpawnBackgroundElementCoroutine());
            }

            _lineRenderer.transform.position = gondola.transform.position;
            _lineRenderer.materials[0].SetFloat("_Scroll", gondola.transform.position.z);

        }

        if (Machine.IsInState(TestCutsceneFsmState.InteractableChannel1))
        {
            var pos = _minorSpline.EvaluatePosition(Util.SmoothLerp01(1f - (TimeInCurrentState() / 1f)));
            _interactableParticles.transform.position = Vector3.Lerp(_interactableParticles.transform.position, pos, Time.deltaTime * 15f);
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.InteractableChannel2))
        {
            var pos = _minorSpline.EvaluatePosition(Util.SmoothLerp01(TimeInCurrentState() / 1f));
            _interactableParticles.transform.position = Vector3.Lerp(_interactableParticles.transform.position, pos, Time.deltaTime * 15f);
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.Shake1))
        {
            _lineRenderer.transform.position = gondola.transform.position;
            // gondola.transform.position += gondola.forward * (Mathf.Lerp(10f, 0f, Mathf.InverseLerp(0, 1.5f, TimeInCurrentState())) * Time.deltaTime);
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.Channel))
        {
            _lineRenderer.transform.position = gondola.transform.position;

            var w = (TimeInCurrentState() - 1.5f) / 2f;
            _channelDolly.m_PathPosition = Util.SmoothLerp01(w);
            _interactableParticles.transform.position = _channelSpline.EvaluatePosition(w);
        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown1))
        {
            gondola.transform.position += gondola.up * (TimeInCurrentState() * TimeInCurrentState() * -100f * Time.deltaTime);
            Time.timeScale = Mathf.Lerp(1f, 0.01f, Mathf.InverseLerp(0.55f, 0.725f, TimeInCurrentState()));
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeScale", Mathf.Lerp(1f, 0.01f, Mathf.InverseLerp(0, 0.725f, TimeInCurrentState())));

        }
        
        if (Machine.IsInState(TestCutsceneFsmState.MoveCubeDown2))
        {
            gondola.transform.position += gondola.up * (-60f * Time.deltaTime);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeScale", Mathf.Lerp(0.01f, 1f, Mathf.InverseLerp(0, 0.125f, TimeInCurrentState())));
        }

        if (Machine.IsInState(TestCutsceneFsmState.Shake2))
        {
            // Time.timeScale = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(0.2f, 0.5f, TimeInCurrentState()));
        }
    }

    protected override void OnStartComplete()
    {
        base.OnStartComplete();
        FMODSceneManager.Singleton.Play(FMODSceneManager.FMODSceneEvent.WindAmbience);
        if (SaveSystem.GetPersistentEventCompleted(CutscenePersistentEvent))
        {
            Machine.Jump(CutsceneFsmState.Inactive);
            return;
        }
        Machine.Fire(CutsceneFsmTrigger.StartCutscene);
    }

    protected override void OnStateChanged(TriggerParams triggerParams)
    {
        base.OnStateChanged(triggerParams);
        _stateGondolaStartingPosition = gondola.position;
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        IntroCutsceneTrigger.OnEnter += OnTrigger;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        IntroCutsceneTrigger.OnEnter -= OnTrigger;
    }
}
