using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{
    
    private void PitonHomingOnUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _currentPitonTransform.position + PitonTargetOffset, Time.deltaTime * 10f);
        
        if (Mathf.Abs(transform.position.y - (_currentPitonTransform.position.y + PitonTargetOffset.y)) < 0.5f) Machine.Fire(PlayerFsmTrigger.ArriveAtPiton);
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
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.RespectParentTransform);

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
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            // .Permit(PlayerFsmTrigger.EnterPitonTrigger, PlayerFsmState.PitonHoming) // TODO
            // .SubstateOf(PlayerFsmState.WallInteractable)
            .OnEntry(_ =>
            {
                _momentum = 5;
                YVelocity = 36f;
                _currentPitonTransform.DOShakePosition(0.5f, 0.25f, 20);
            });
    }
}