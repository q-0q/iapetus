using UnityEngine;

public partial class PlayerFsm
{
    private void GroundMoveOnUpdate()
    {
        HandleInputMomentumChange();
        HandleTurning();
        HandleCollisionMove();

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void GroundMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                ReplaceAnimatorTrigger("GroundMove");
            });
    }
}