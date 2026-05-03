using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract partial class CultistFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CultistFsmState.Idle);
        
        Machine.Configure(CultistFsmState.Give)
            .Permit(FsmTrigger.Timeout, CultistFsmState.Idle);

        Machine.Configure(CultistFsmState.Dancing);
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(CultistFsmState.Give, 4f);
        
        StateMapConfig.AnimationTrigger.Add(CultistFsmState.Idle, "Idle");
        StateMapConfig.AnimationTrigger.Add(CultistFsmState.Dancing, "Dance");
        StateMapConfig.AnimationTrigger.Add(CultistFsmState.Give, "Give");
    }
}