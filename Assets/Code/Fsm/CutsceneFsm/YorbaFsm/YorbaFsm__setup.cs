using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class YorbaFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(YorbaFsmState.Idle)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.SpeakingDefault);
        
        Machine.Configure(YorbaFsmState.SpeakingDefault)
            .OnEntry(_ => print("test"))
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.Idle);
        
        Machine.Configure(YorbaFsmState.SpeakingQuestReady)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

        Machine.Configure(YorbaFsmState.QuestChannel)
            .Permit(FsmTrigger.Timeout, YorbaFsmState.Idle)
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
        
        StateMapConfig.Duration.Add(YorbaFsmState.QuestChannel, 10f);
        
        
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Idle, "Idle");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingDefault, "Speak");
        // StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(YorbaFsmState.QuestChannel, "Channel");
    }
}