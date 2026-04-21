using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SnailHunterFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(SnailHunterFsmState.Idle)
            .Permit(SnailHunterFsmTrigger.OnInteracted, SnailHunterFsmState.SpeakingDefault);
        
        Machine.Configure(SnailHunterFsmState.SpeakingDefault)
            .Permit(SnailHunterFsmTrigger.OnDialogueCompleted, SnailHunterFsmState.Idle);
        
        Machine.Configure(SnailHunterFsmState.SpeakingQuestReady)
            .Permit(SnailHunterFsmTrigger.OnDialogueCompleted, SnailHunterFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

        Machine.Configure(SnailHunterFsmState.QuestChannel)
            .Permit(FsmTrigger.Timeout, SnailHunterFsmState.Idle)
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
        
        StateMapConfig.Duration.Add(SnailHunterFsmState.QuestChannel, 10f);
        
        
        // StateMapConfig.AnimationTrigger.Add(SnailHunterFsmState.Idle, "Eating");
        // StateMapConfig.AnimationTrigger.Add(SnailHunterFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(SnailHunterFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(SnailHunterFsmState.QuestChannel, "Channel");
    }
}