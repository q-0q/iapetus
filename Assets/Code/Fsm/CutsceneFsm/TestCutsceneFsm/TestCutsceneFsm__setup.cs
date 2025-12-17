using DG.Tweening;
using UnityEngine;

public partial class TestCutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                Time.timeScale = 1f;
                _virtualCamera.Priority = 0;
            })
            .Permit(CutsceneFsmTrigger.StartCutscene, TestCutsceneFsmState.AlignCamera);

        Machine.Configure(TestCutsceneFsmState.AlignCamera)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeForward)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _canvasGroup.alpha = 1f;
                _virtualCamera.Priority = 20;
            });
        
        Machine.Configure(TestCutsceneFsmState.MoveCubeForward)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.Shake1)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.Shake1)
            .Permit(TestCutsceneFsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeDown1)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                cube.DOShakePosition(0.75f, 0.5f);
            });

        Machine.Configure(TestCutsceneFsmState.MoveCubeDown1)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.WaitForInput)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.WaitForInput)
            .Permit(TestCutsceneFsmTrigger.PlayerInputJump, TestCutsceneFsmState.WaitForJumpsquat)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.WaitForJumpsquat)
            .Permit(TestCutsceneFsmTrigger.PlayerInJumpState, TestCutsceneFsmState.MoveCubeDown2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                Time.timeScale = 0.5f;
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Jumpsquat);
            });

        Machine.Configure(TestCutsceneFsmState.MoveCubeDown2)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.Shake2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                Time.timeScale = 1f;
            });
        
        Machine.Configure(TestCutsceneFsmState.Shake2)
            .Permit(TestCutsceneFsmTrigger.Timeout, TestCutsceneFsmState.Inactive)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                // HitstopManager.Singleton.StartHitstop(0.2f);
                cube.DOShakePosition(1.5f, 1f);
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeForward, 3f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown1, 0.65f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown2, 0.4f);
        
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.MoveCubeDown2, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.Shake2, false);
        
    }
}