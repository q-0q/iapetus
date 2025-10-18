using UnityEngine;

public partial class PlayerFsm
{
    private void GroundMoveOnUpdate()
    {
        HandleInputMomentumChange();
        HandleTurning();
        HandleCollisionMove();

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void GroundMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            // .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
            });
    }
}