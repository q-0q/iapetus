using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SavapheFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive);

        Machine.Configure(SavapheFsmState.NotCrossed)
            .SubstateOf(CutsceneFsmState.Inactive)
            .Permit(SavapheFsmTrigger.PlayerCrossed, SavapheFsmState.Crossing1);


        Machine.Configure(SavapheFsmState.Crossing1)
            .Permit(FsmTrigger.Timeout, SavapheFsmState.Crossing2)
            .SubstateOf(CutsceneFsmState.Active);

        Machine.Configure(SavapheFsmState.Crossing2)
            .Permit(FsmTrigger.Timeout, SavapheFsmState.Crossing3)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _virtualCamera.Priority = 20;
            });
        
        Machine.Configure(SavapheFsmState.Crossing3)
            .Permit(FsmTrigger.Timeout, SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Active)
            .OnExit(_ =>
            {
                _virtualCamera.Priority = -10;
            });
        
        Machine.Configure(SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
        {

        });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing1, 1.5f);
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing2, 1.5f);
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing3, 3f);
    }
}