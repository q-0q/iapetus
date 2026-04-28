using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wasp;

public partial class YorbaFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(YorbaFsmState.Hidden)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.Revealing);

        Machine.Configure(YorbaFsmState.Revealing)
            .Permit(FsmTrigger.Timeout, YorbaFsmState.SpeakingDefault)
            .PermitIf(FsmTrigger.Timeout, YorbaFsmState.SpeakingQuestReady, QuestReadyClause, 1);
        
        Machine.Configure(YorbaFsmState.SpeakingDefault)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.Idle)
            .OnEntry(_ =>
            {
                if (_dialogueController.currentDialogueIndex >= 3) return;
                var items = SaveSystem.GetAllItems();
                if (items.Contains("ErhuFragment1") || items.Contains("ErhuFragment2") ||
                       items.Contains("ErhuFragment3")) _dialogueController.currentDialogueIndex = 3;
            });
        
        Machine.Configure(YorbaFsmState.Idle)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.SpeakingDefault)
            .PermitIf(YorbaFsmTrigger.OnInteracted, YorbaFsmState.SpeakingQuestReady, QuestReadyClause, 1)
            .OnEntry(_ =>
            {
                _dialogueController.canvasDelay = 0;
                _interactable.text = "Speak";
            });
        
        Machine.Configure(YorbaFsmState.SpeakingQuestReady)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.Idle)
            .OnEntry(_ =>
            {
                _dialogueController.currentDialogueIndex = 5;
            })
            .OnExit(_ =>
            {
                SaveSystem.WritePersistentEvent(PersistentEvent);
                SaveSystem.RemoveItem("ErhuFragment1");
                SaveSystem.RemoveItem("ErhuFragment2");
                SaveSystem.RemoveItem("ErhuFragment3");
                SaveSystem.WritePlayerInGamePosition(PlayerFsm.Singleton.transform.position, "", PlayerFsm.Singleton.transform.rotation.eulerAngles.y);
                SceneLoader.Singleton.LoadScene(SceneManager.GetActiveScene().name);
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
        
        Machine.Configure(YorbaFsmState.InstrumentIdle)
            .Permit(YorbaFsmTrigger.OnInteracted, YorbaFsmState.InstrumentSpeak);

        Machine.Configure(YorbaFsmState.InstrumentSpeak)
            .Permit(YorbaFsmTrigger.OnDialogueCompleted, YorbaFsmState.InstrumentIdle);

    }

    private bool QuestReadyClause(TriggerParams _)
    {
        var items = SaveSystem.GetAllItems();
        return (items.Contains("ErhuFragment1") && items.Contains("ErhuFragment2") && items.Contains("ErhuFragment3"));
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(YorbaFsmState.QuestChannel, 10f);
        StateMapConfig.Duration.Add(YorbaFsmState.Revealing, 2.9f);
        
        
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Hidden, "Hidden");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Revealing, "Speak");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingDefault, "SpeakLoop");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.SpeakingQuestReady, "SpeakLoop");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.Idle, "Idle");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.InstrumentIdle, "InstrumentIdle");
        StateMapConfig.AnimationTrigger.Add(YorbaFsmState.InstrumentSpeak, "InstrumentSpeak");
        
        // StateMapConfig.AnimationTrigger.Add(YorbaFsmState.QuestChannel, "Channel");
    }
}