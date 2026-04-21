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

public partial class YorbaFsm : CutsceneFsm
{
    public class YorbaFsmState : CutsceneFsmState
    {
        public static int Hidden;
        public static int SpeakingDefault;
        public static int SpeakingQuestReady;
        public static int QuestChannel;
        public static int Idle;
        public static int Revealing;
    }

    public class YorbaFsmTrigger : CutsceneFsm.CutsceneFsmTrigger
    {
        public static int OnInteracted;
        public static int OnDialogueCompleted;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        _dialogueController = GetComponentInChildren<DialogueController>();
        Animator = GetComponentInChildren<Animator>();
        _fakeEyesRenderer = transform.Find("yorba").Find("FakeEyes").GetComponent<SkinnedMeshRenderer>();
        _light = GetComponentInChildren<Light>();
        if (SaveSystem.GetPersistentEventCompleted(ExpositionPersistentEvent))
            _dialogueController.currentDialogueIndex = 1;

    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = YorbaFsmState.Hidden;
        Shader.SetGlobalVector("_YorbaFakeLightPosition", _light.transform.position);
        Shader.SetGlobalFloat("_YorbaFakeLightFalloff", 3f);
        Shader.SetGlobalFloat("_YorbaFakeLightDistance", 0f);
        
        print(Shader.GetGlobalVector("_YorbaFakeLightPosition"));
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(YorbaFsmState.Hidden))
        {
            var lerpStrength = Time.deltaTime * 10f;
            var newLightDistance = Mathf.Lerp(Shader.GetGlobalFloat("_YorbaFakeLightDistance"), 0, lerpStrength);
            Shader.SetGlobalFloat("_YorbaFakeLightDistance", newLightDistance);

            _fakeEyesRenderer.material.SetFloat("_Alpha",
                Mathf.Lerp(_fakeEyesRenderer.material.GetFloat("_Alpha"), 0f, Time.deltaTime * 5f));
        }
        else
        {
            var weightLerpStrength =
                Time.deltaTime * Mathf.Lerp(0.5f, 1.25f, Mathf.InverseLerp(0, 0.75f, TimeInCurrentState()));


            var lightLerpStrength =
                Time.deltaTime * Mathf.Lerp(0.2f, 0.3f, Mathf.InverseLerp(0, 1.25f, TimeInCurrentState()));
            var newLightDistance = Mathf.Lerp(Shader.GetGlobalFloat("_YorbaFakeLightDistance"), 20f, lightLerpStrength);
            Shader.SetGlobalVector("_YorbaFakeLightPosition", _light.transform.position);
            Shader.SetGlobalFloat("_YorbaFakeLightDistance", newLightDistance);

            _fakeEyesRenderer.material.SetFloat("_Alpha",
                Mathf.Lerp(_fakeEyesRenderer.material.GetFloat("_Alpha"), 1f, Time.deltaTime * 5f));
        }
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
        _interactable.OnInteracted += OnInteracted;
        _dialogueController.OnCompleted += OnDialogueCompleted;
        _dialogueController.OnProgressed += OnDialogueProgressed;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueController.OnCompleted -= OnDialogueCompleted;
        _dialogueController.OnProgressed -= OnDialogueProgressed;
    }
}
