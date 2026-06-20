using System.Collections;
using System.Linq;
using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util = Code.Misc.Util;

public partial class ProfessorFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(ProfessorFsmState.Busy)
            .PermitIf(ProfessorFsmTrigger.OnNodeCompleted, ProfessorFsmState.Shocked, _ => true)
            .OnExitFrom(ProfessorFsmTrigger.OnNodeCompleted, _ =>
            {
                var map = SaveSystem.GetAllItems().Contains("Map");
                _dialogueController.currentDialogueIndex = map ? 2 : 1;
                _dialogueController.DialogueName = "Silicant Diamber";
                _interactable.text = "Speak";
                if (!map)
                {
                    _halo.SetActive(true);
                }
            });

        Machine.Configure(ProfessorFsmState.Shocked)
            .PermitIf(ProfessorFsmTrigger.OnInteracted, ProfessorFsmState.ShockedToSpeakingMural, _ => true)
            .OnEntry(_ =>
            {
                _dialogueController.canvasDelay = 1.75f;
            });


        Machine.Configure(ProfessorFsmState.ShockedToSpeakingMural)
            .PermitIf(FsmTrigger.Timeout, ProfessorFsmState.SpeakingMural, _ => true)
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                ReplaceAnimatorTrigger("SpeakingMural"); // doing it this way prevents animation from retriggering when we dont want it to
                
                

            })
            .SubstateOf(ProfessorFsmState.Speaking);;
        
        Machine.Configure(ProfessorFsmState.SpeakingMural)
            .PermitIf(ProfessorFsmTrigger.OnDialogueCompleted, ProfessorFsmState.MuralIdle, _ => true)
            .SubstateOf(ProfessorFsmState.Speaking)
            .OnExit(_ =>
            {
                if (SaveSystem.GetAllItems().Contains("Map")) return;
                SaveSystem.WriteItem("Map");
                SaveSystem.WritePersistentEvent("Map");
                AcquisitionCanvas.Singleton.InvokeMapAcquisition();
                
                StartCoroutine(TutorialCoroutine());
                
                StartCoroutine(HaloCoroutine());
                IEnumerator HaloCoroutine()
                {
                    var t = 0f;
                    var d = 2f;
                    var renderer = _halo.GetComponent<Renderer>();
                    _halo.GetComponentInChildren<ParticleSystem>().Stop();
                    while (t < d)
                    {
                        var w = Util.SmoothLerp01(t / d);
                        renderer.material.SetFloat("_Dot", Mathf.Lerp(1f, 0, w));
                        renderer.material.SetFloat("_FresnelDepth", Mathf.Lerp(15f, 0, w));
                        t += Time.deltaTime;
                        yield return null;
                    }
                    _halo.SetActive(false);
                    
                }
                
                IEnumerator TutorialCoroutine()
                {
                    yield return new WaitForSeconds(11f);
                    TutorialCanvas.Singleton.ShowTutorialText("Open map", "Map");
                }


            });

        Machine.Configure(ProfessorFsmState.MuralIdle)
            .PermitIf(ProfessorFsmTrigger.OnInteracted, ProfessorFsmState.SpeakingMural, _ => true)
            .PermitIf(FsmTrigger.Timeout, ProfessorFsmState.Shocked, _ => true)
            .SubstateOf(ProfessorFsmState.Speaking)
            .OnEntry(_ =>
            {
                _dialogueController.canvasDelay = 0;
            });


    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(ProfessorFsmState.ShockedToSpeakingMural, 2f);
        StateMapConfig.Duration.Add(ProfessorFsmState.MuralIdle, 4.5f);
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.Busy, "Busy");
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.Shocked, "Shocked");
        // StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.SpeakingMural, "SpeakingMural");
        // StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.MuralIdle, "SpeakingMural"); 
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.ShockedToSpeakingMural, "ShockedToSpeakingMural");
        
    }
}