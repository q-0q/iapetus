using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{
    
    private void PitonHomingOnUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _currentPitonTransform.position + PitonTargetOffset, Time.deltaTime * 5f);
    }

    private void PitonsquatOnUpdate()
    {
        transform.position = _currentPitonTransform.position;
    }

    private void PitonConfigure()
    {

        Machine.Configure(PlayerFsmState.PitonHoming)
            .Permit(PlayerFsmTrigger.ArriveAtPiton, PlayerFsmState.PitonFlipsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.PitonFlipsquat)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .OnEntry(@params =>
            {
                if (@params is not PitonParam pitonParam) return;
                _currentPitonTransform = pitonParam.Piton;
            } );

        Machine.Configure(PlayerFsmState.Pitonsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.PitonFlipsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall);

        Machine.Configure(PlayerFsmState.PitonFlipsquat)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.PitonFlip)
            .OnEntry(_ =>
            {
                // transform.position = _currentPitonTransform.position;
                _currentPitonTransform.DOShakePosition(0.1f, 0.15f, 20);
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
            });
        
        Machine.Configure(PlayerFsmState.PitonFlip)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            // .SubstateOf(PlayerFsmState.WallInteractable)
            .OnEntry(_ =>
            {
                _momentum = 5;
                YVelocity = 36f;
                _currentPitonTransform.DOShakePosition(0.5f, 0.25f, 20);
            });

        Machine.Configure(PlayerFsmState.PitonInteractable)
            .PermitIf(PlayerFsmTrigger.EnterPitonTrigger, PlayerFsmState.PitonHoming, _=> YVelocity < 20f);
        
    }
}