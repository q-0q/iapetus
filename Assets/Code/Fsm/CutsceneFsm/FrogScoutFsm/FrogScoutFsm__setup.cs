using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class FrogScoutFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(FrogScoutFsmState.Idle)
            .Permit(FrogScoutFsmTrigger.OnInteracted, FrogScoutFsmState.SpeakingDefault);
        
        Machine.Configure(FrogScoutFsmState.SpeakingDefault)
            .Permit(FrogScoutFsmTrigger.OnDialogueCompleted, FrogScoutFsmState.Idle);
        
        Machine.Configure(FrogScoutFsmState.SpeakingQuestReady)
            .Permit(FrogScoutFsmTrigger.OnDialogueCompleted, FrogScoutFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

        Machine.Configure(FrogScoutFsmState.QuestChannel)
            .Permit(FsmTrigger.Timeout, FrogScoutFsmState.Idle)
            .OnExit(_ =>
            {

            })
            .OnEntry(_ =>
            {
                CutsceneManager.Singleton.SetPseudoCutsceneActive();
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(FrogScoutFsmState.QuestChannel, 10f);
        
        
        // StateMapConfig.AnimationTrigger.Add(FrogScoutFsmState.Idle, "Eating");
        // StateMapConfig.AnimationTrigger.Add(FrogScoutFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(FrogScoutFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(FrogScoutFsmState.QuestChannel, "Channel");
    }
}