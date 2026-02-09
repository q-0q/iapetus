using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SavapheFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                _virtualCamera.Priority = 0;
            })
            .Permit(CutsceneFsmTrigger.StartCutscene, SavapheFsmState.Crossing);

        Machine.Configure(SavapheFsmState.NotCrossed)
            .SubstateOf(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                
            });
        
        Machine.Configure(SavapheFsmState.Crossing)
            .Permit(FsmTrigger.Timeout, SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
        {

        });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing, 3f);
    }
}