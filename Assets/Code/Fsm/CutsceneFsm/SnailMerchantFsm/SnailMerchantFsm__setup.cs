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
            .PermitIf(SnailMerchantFsmTrigger.OnInteracted, SnailMerchantFsmState.SpeakingQuestReady, _ => IsBitRequirementMet() && _dialogueController.currentDialogueIndex >= 4, 2)
            .PermitIf(SnailMerchantFsmTrigger.OnInteracted, SnailMerchantFsmState.SpeakingQuestComplete, _ => SaveSystem.GetPersistentEventCompleted(PersistentEvent), 3);;
        
        Machine.Configure(SnailMerchantFsmState.SpeakingDefault)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.Idle);
        
        Machine.Configure(SnailMerchantFsmState.SpeakingQuestReady)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                _dialogueController.currentDialogueIndex = 5;
            });
        
        Machine.Configure(SnailMerchantFsmState.SpeakingQuestComplete)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                _dialogueController.currentDialogueIndex = 6;
            });

        Machine.Configure(SnailMerchantFsmState.QuestChannel)
            .Permit(FsmTrigger.Timeout, SnailMerchantFsmState.Idle)
            .OnExit(_ =>
            {
                SaveSystem.WritePlayerInGamePosition(Vector3.zero, "Entrance", 0, 0);
                SceneLoader.Singleton.LoadScene("C1-Snail");
            })
            .OnEntry(_ =>
            {
                CutsceneManager.Singleton.SetPseudoCutsceneActive();
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference("event:/SnailChannel"), gameObject);
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(SnailMerchantFsmState.QuestChannel, 10f);
        
        
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.Idle, "Eating");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.SpeakingDefault, "Stand");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.SpeakingQuestReady, "Stand");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.SpeakingQuestComplete, "Stand");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.QuestChannel, "Channel");
    }
}