using System.Linq;
using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ProfessorFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(ProfessorFsmState.Busy)
            .PermitIf(ProfessorFsmTrigger.OnNodeCompleted, ProfessorFsmState.Shocked, _ => true)
            .OnExitFrom(ProfessorFsmTrigger.OnNodeCompleted, _ =>
            {
                _dialogueController.currentDialogueIndex = 1;
            });

        Machine.Configure(ProfessorFsmState.Shocked)
            .PermitIf(ProfessorFsmTrigger.OnInteracted, ProfessorFsmState.ShockedToSpeakingMural, _ => true)
            .OnEntry(_ =>
            {
                
            });


        Machine.Configure(ProfessorFsmState.ShockedToSpeakingMural)
            .PermitIf(FsmTrigger.Timeout, ProfessorFsmState.SpeakingMural, _ => true)
            .SubstateOf(ProfessorFsmState.Speaking);;
        
        Machine.Configure(ProfessorFsmState.SpeakingMural)
            .PermitIf(ProfessorFsmTrigger.OnDialogueCompleted, ProfessorFsmState.Shocked, _ => true)
            .SubstateOf(ProfessorFsmState.Speaking)
            .OnExit(_ =>
            {
                if (SaveSystem.GetAllItems().Contains("Map")) return;
                SaveSystem.WriteItem("Map");
                SaveSystem.WritePersistentEvent("Map");
                KeyItem.InvokeKeyItemCollected("Map");
            });
        

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(ProfessorFsmState.ShockedToSpeakingMural, 2.5f);
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.Busy, "Busy");
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.Shocked, "Shocked");
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.SpeakingMural, "SpeakingMural");
        StateMapConfig.AnimationTrigger.Add(ProfessorFsmState.ShockedToSpeakingMural, "ShockedToSpeakingMural");
        
    }
}