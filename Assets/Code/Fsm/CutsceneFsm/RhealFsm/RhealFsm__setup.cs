using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class RhealFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(RhealFsmState.Idle)
            .Permit(RhealFsmTrigger.OnInteracted, RhealFsmState.Speaking);
        
        Machine.Configure(RhealFsmState.Idle)
            .Permit(RhealFsmTrigger.OnDialogueCompleted, RhealFsmState.Idle);
        

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.AnimationTrigger.Add(RhealFsmState.Idle, "Stand");
        // StateMapConfig.AnimationTrigger.Add(RhealFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(RhealFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(RhealFsmState.QuestChannel, "Channel");
    }
}