using UnityEngine;

public partial class PlayerFsm
{
    private void GroundMoveOnUpdate()
    { 
        if (_playerInput.actions["Sprint"].IsPressed()) isSprinting = true;
        
        HandleInputMomentumChange();
        HandleTurning(1f, false, 1f, false, isSprinting ? 0.5f : 1f);
        HandleCollisionMove();

        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
        

    }

    private void GroundMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(PlayerFsm.PlayerFsmTrigger.IdleMomentumThresholdPassedDecelerating, PlayerFsm.PlayerFsmState.StepEnd)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn, _=> _momentum > HardTurnMinimumMomentum)
            // .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            // .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Slide, _ => _slopeTimer > 0.2f)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.TightropeMove, IsTightropeTrigger, 6)
            .SubstateOf(PlayerFsmState.SlideInteractable)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
            });

        Machine.Configure(PlayerFsmState.GroundMoveAfterVault)
            .SubstateOf(PlayerFsmState.GroundMove);
    }
}