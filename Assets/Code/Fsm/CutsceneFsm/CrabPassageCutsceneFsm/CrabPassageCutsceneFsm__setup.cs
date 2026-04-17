using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CrabPassageCutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CrabPassageCutsceneFsmState.IdleDefault)
            .Permit(CrabPassageCutsceneFsmTrigger.OnInteracted, CrabPassageCutsceneFsmState.SpeakingDefault);
        
        Machine.Configure(CrabPassageCutsceneFsmState.SpeakingDefault)
            .Permit(CrabPassageCutsceneFsmTrigger.OnDialogueCompleted, CrabPassageCutsceneFsmState.IdleDefault);
        
        Machine.Configure(CrabPassageCutsceneFsmState.SpeakingQuestComplete)
            .Permit(CrabPassageCutsceneFsmTrigger.OnDialogueCompleted, CrabPassageCutsceneFsmState.IdleDefault)
            .OnEntry(_ =>
            {
                // _dialogueController.currentDialogueIndex = ???;
            });
        

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.Idle, "Eating");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.QuestChannel, "Channel");
    }
}