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
                _virtualCameraA.Priority = 20;
            })
            .OnExit(_ =>
            {
                _virtualCameraA.Priority = -10;
            });
        
        Machine.Configure(SavapheFsmState.Crossing3)
            .Permit(FsmTrigger.Timeout, SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _marker.position = _endPosition.position;
                _marker.rotation = _endPosition.rotation;
                _virtualCameraB.Priority = 20;
            })
            .OnExit(_ =>
            {
                _virtualCameraB.Priority = -10;
            });
        
        Machine.Configure(SavapheFsmState.Crossed)
            .SubstateOf(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            { 
                _notCrossedDialogue.gameObject.SetActive(false);
                _crossedDialogue.gameObject.SetActive(true);
                
                _marker.position = _endPosition.position;
                _marker.rotation = _endPosition.rotation;
                SaveSystem.WritePersistentEvent(CutscenePersistentEvent, 0);
        });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.AnimationTrigger.Add(SavapheFsmState.NotCrossed, "NotCrossedIdle");
        StateMapConfig.AnimationTrigger.Add(SavapheFsmState.Crossing2, "CrossingA");
        StateMapConfig.AnimationTrigger.Add(SavapheFsmState.Crossing3, "CrossingB");
        StateMapConfig.AnimationTrigger.Add(SavapheFsmState.Crossed, "Crossed");
        
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing1, 1.5f);
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing2, 1.5f);
        StateMapConfig.Duration.Add(SavapheFsmState.Crossing3, 3f);
    }
}