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
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.ShowText)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                PlayerFsm.Singleton.transform.position = _playerTransformOnStart.position;
                PlayerFsm.Singleton.transform.rotation = _playerTransformOnStart.rotation;
                _mainCanvasGroup.alpha = 1f;
                _virtualCamera.Priority = 20;
                _currentTextId = 0;
            });
        
        Machine.Configure(TestCutsceneFsmState.ShowText)
            .Permit(TestCutsceneFsmTrigger.TextComplete, TestCutsceneFsmState.TextFade)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.TextFade)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeForward)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.MoveCubeForward)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.Shake1)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.Shake1)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeDown1)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                gondola.DOShakePosition(0.75f, 0.5f);
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
                gondola.DOShakePosition(1.5f, 1f);
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(TestCutsceneFsmState.TextFade, 3f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeForward, 9f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.Shake1, 2f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown1, 0.65f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown2, 0.4f);
        
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.MoveCubeDown2, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.Shake2, false);
        
    }
}