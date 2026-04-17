using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CrabGuardFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CrabGuardFsmState.IdleDefault)
            .Permit(CrabGuardFsmTrigger.OnInteracted, CrabGuardFsmState.SpeakingDefault);
        
        Machine.Configure(CrabGuardFsmState.SpeakingDefault)
            .Permit(CrabGuardFsmTrigger.OnDialogueCompleted, CrabGuardFsmState.IdleDefault);
        
        Machine.Configure(CrabGuardFsmState.SpeakingQuestComplete)
            .Permit(CrabGuardFsmTrigger.OnDialogueCompleted, CrabGuardFsmState.IdleDefault)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.Idle, "Eating");
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.QuestChannel, "Channel");
    }
}