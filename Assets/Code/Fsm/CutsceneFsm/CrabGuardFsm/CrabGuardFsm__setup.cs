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
        
        Machine.Configure(CrabGuardFsmState.IdleQuestComplete)
            .Permit(CrabGuardFsmTrigger.OnInteracted, CrabGuardFsmState.SpeakingQuestComplete);
        
        Machine.Configure(CrabGuardFsmState.SpeakingQuestComplete)
            .Permit(CrabGuardFsmTrigger.OnDialogueCompleted, CrabGuardFsmState.IdleQuestComplete)
            .OnEntry(_ =>
            {
                if (_dialogueController.currentDialogueIndex < questCompleteDialogueIndex) _dialogueController.currentDialogueIndex = questCompleteDialogueIndex;
            });
        

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.IdleDefault, "Idle");
        StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.IdleQuestComplete, "Dancing");
        StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.SpeakingQuestComplete, "Idle");
        
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabGuardFsmState.QuestChannel, "Channel");
    }
}