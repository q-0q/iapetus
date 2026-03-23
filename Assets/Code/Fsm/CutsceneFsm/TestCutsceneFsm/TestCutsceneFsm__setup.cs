using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util = Code.Misc.Util;

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
                _finalVirtualCamera.Priority = 0;
                _mainCanvasGroup.alpha = 0f;
                gondola.transform.position = _endPosition.position;
            })
            .Permit(CutsceneFsmTrigger.StartCutscene, TestCutsceneFsmState.AlignCamera);

        Machine.Configure(TestCutsceneFsmState.AlignCamera)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.CanvasFade) // ShowText
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                PlayerFsm.Singleton.transform.position = _playerTransformOnStart.position;
                PlayerFsm.Singleton.transform.rotation = _playerTransformOnStart.rotation;
                _mainCanvasGroup.alpha = 1f;
                
                _currentTextId = 0;
                _moveCubeForwardShake1 = false;
                _moveCubeForwardShake2 = false;
                
                PlayerCinemachineFreeLook.Singleton.SetAxes(-45f, 0.7f);
            });
        
        Machine.Configure(TestCutsceneFsmState.ShowText)
            .Permit(TestCutsceneFsmTrigger.TextComplete, TestCutsceneFsmState.TextFade)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.TextFade)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.CanvasFade)
            .Permit(CutsceneFsmTrigger.Skip, TestCutsceneFsmState.Shake2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
        {
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneIdle);
            _creakEventInstance = FMODUnity.RuntimeManager.CreateInstance(gondolaCreakEventReference);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_creakEventInstance, gondola.gameObject);
            _creakEventInstance.start();
        });

        Machine.Configure(TestCutsceneFsmState.CanvasFade)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.PlayerControl)
            .SubstateOf(TestCutsceneFsmState.MoveForward)
            // .Permit(CutsceneFsmTrigger.Skip, TestCutsceneFsmState.Shake2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                innerCube.DOShakePosition(3.75f, 0.25f);
                innerCube.DOShakeRotation(3.75f, 0.5f);
            });
        
        Machine.Configure(TestCutsceneFsmState.PlayerControl)
            .SubstateOf(TestCutsceneFsmState.MoveForward)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.InteractableReady)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                innerCube.DOShakePosition(3.75f, 0.25f);
                innerCube.DOShakeRotation(3.75f, 0.5f);
            });
        
        Machine.Configure(TestCutsceneFsmState.InteractableReady)
            .SubstateOf(TestCutsceneFsmState.MoveForward)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _interactable.SetEnabled(true);
                _interactableParticles.Play();
                Util.InvokeSphereEffect(_interactableParticles.transform.position - Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
                innerCube.DOShakePosition(3.75f, 0.25f);
                innerCube.DOShakeRotation(3.75f, 0.5f);
            });
        
        Machine.Configure(TestCutsceneFsmState.Shake1)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeDown1)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _virtualCamera.Priority = 20;
                _creakEventInstance.stop(STOP_MODE.IMMEDIATE);
                FMODUnity.RuntimeManager.PlayOneShotAttached(gondolaMinorBangEventReference, gondola.gameObject);
                innerCube.DOShakePosition(0.75f, 0.5f);
                innerCube.DOShakeRotation(3.75f, 0.5f);
            });

        Machine.Configure(TestCutsceneFsmState.MoveCubeDown1)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.WaitForInput)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(gondolaBreakEventReference, gondola.gameObject);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneWary);
                innerCube.DOShakePosition(0.75f, 0.5f);
            });
        
        Machine.Configure(TestCutsceneFsmState.WaitForInput)
            .Permit(TestCutsceneFsmTrigger.PlayerInputJump, TestCutsceneFsmState.WaitForJumpsquat)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                TutorialCanvas.Singleton.ShowTutorialText("South button / space: Jump");
            });
        
        Machine.Configure(TestCutsceneFsmState.WaitForJumpsquat)
            .Permit(TestCutsceneFsmTrigger.PlayerInJumpState, TestCutsceneFsmState.MoveCubeDown2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                TutorialCanvas.Singleton.HideTutorialText();
                Time.timeScale = 0.5f;
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Jumpsquat);
            });

        Machine.Configure(TestCutsceneFsmState.MoveCubeDown2)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.Shake2)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                _finalVirtualCamera.Priority = 20;
                _virtualCamera.Priority = 0;
                Time.timeScale = 0.65f;
            });
        
        Machine.Configure(TestCutsceneFsmState.Shake2)
            .Permit(TestCutsceneFsmTrigger.Timeout, TestCutsceneFsmState.Inactive)
            .SubstateOf(CutsceneFsmState.Active)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(gondolaCrashEventReference, gondola.gameObject);

                if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.CutsceneIdle))
                {
                    PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.HardLand);
                }
                // _finalVirtualCamera.Priority = 0;
                // _virtualCamera.Priority = 20;
                Time.timeScale = 1f;
                _creakEventInstance.stop(STOP_MODE.IMMEDIATE);
                _mainCanvasGroup.alpha = 0f;
                _impactParticles.Play(true);
                innerCube.DOShakePosition(1.5f, 1f);
                gondola.DOShakePosition(1.5f, 1f);
                gondola.transform.position = _endPosition.position;
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeScale", 1f);
                // SaveSystem.WritePersistentEvent(CutscenePersistentEvent);
                // SaveSystem.WritePlayerInGamePosition(_endPosition.position + Vector3.up * 5f, "", 0f);
            });
        
        // Machine.Configure(TestCutsceneFsmState.FinalCamera)
        //     .Permit(TestCutsceneFsmTrigger.Timeout, TestCutsceneFsmState.Inactive)
        //     .SubstateOf(CutsceneFsmState.Active)
        //     .OnEntry(_ =>
        //     {
        //         _virtualCamera.Priority = 0;
        //         _finalVirtualCamera.Priority = 20;
        //
        //     });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(TestCutsceneFsmState.TextFade, 3f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.CanvasFade, CanvasFadeDuration);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.Shake1, 3f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.Shake2, 3f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown1, 0.725f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.MoveCubeDown2, 0.625f);
        StateMapConfig.Duration.Add(TestCutsceneFsmState.PlayerControl, 4f);
        
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.PlayerControl, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.InteractableReady, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.MoveCubeDown2, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.Shake2, false);
        StateMapConfig.CutscenePlayerDisabled.Add(TestCutsceneFsmState.FinalCamera, false);
        
        StateMapConfig.IsAbstract.Add(TestCutsceneFsmState.MoveForward, true);
    }
}