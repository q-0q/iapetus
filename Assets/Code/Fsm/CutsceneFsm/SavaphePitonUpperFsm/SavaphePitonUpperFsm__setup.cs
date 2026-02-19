using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SavaphePitonUpperFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive);

        Machine.Configure(SavaphePitonUpperFsmState.NotRung)
            .SubstateOf(CutsceneFsmState.Inactive)
            .Permit(SavaphePitonUpperFsmTrigger.BellRung, SavaphePitonUpperFsmState.Rung);


        Machine.Configure(SavaphePitonUpperFsmState.Rung)
            .SubstateOf(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                GetComponentInChildren<DialogueController>().currentDialogueIndex = 2;
            });
        
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.AnimationTrigger.Add(SavaphePitonUpperFsmState.NotRung, "NotCrossedIdle");
        StateMapConfig.AnimationTrigger.Add(SavaphePitonUpperFsmState.Rung, "NotCrossedDialogue");
    }
}