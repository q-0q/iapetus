using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class YorbaFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(YorbaFsmState.Hidden)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.Revealing);
        
        Machine.Configure(YorbaFsmState.Revealing)
            .Permit(FsmTrigger.Timeout, YorbaFsmState.SpeakingDefault);
        
        Machine.Configure(YorbaFsmState.SpeakingDefault)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.Idle);
        
        Machine.Configure(YorbaFsmState.Idle)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.SpeakingDefault)
            .OnEntry(_ =>
            {
                _dialogueController.canvasDelay = 0;
                _interactable.text = "Speak";
                // _dialogueController.
            });
        
        Machine.Configure(YorbaFsmState.SpeakingQuestReady)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.QuestChannel)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

        Machine.Configure(YorbaFsmState.QuestChannel)
            .Permit(FsmTrigger.Timeout, YorbaFsmState.Hidden)
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
        StateMapConfig.Duration.Add(YorbaFsmState.Revealing, 3f);
        
        
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Hidden, "Hidden");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Revealing, "Speak");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingDefault, "SpeakLoop");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Idle, "Idle");
        // StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(YorbaFsmState.QuestChannel, "Channel");
    }
}