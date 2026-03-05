using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SnailMerchantFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(SnailMerchantFsmState.Idle)
            .Permit(SnailMerchantFsmTrigger.OnInteracted, SnailMerchantFsmState.SpeakingDefault)
            .PermitIf(SnailMerchantFsmTrigger.OnInteracted, SnailMerchantFsmState.SpeakingQuestReady, _ => IsQuestCompleted() && _dialogueController.currentDialogueIndex >= 4, 2);
        
        Machine.Configure(SnailMerchantFsmState.SpeakingDefault)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.Idle);
        
        Machine.Configure(SnailMerchantFsmState.SpeakingQuestReady)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.Idle)
            .OnEntry(_ =>
            {
                BitSystem.Singleton.RemoveBits(500);
                _dialogueController.currentDialogueIndex = 5;
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.Idle, "Eating");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.SpeakingDefault, "Stand");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.SpeakingQuestReady, "Stand");
    }
}