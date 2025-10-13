using UnityEngine;

public partial class PlayerFsm
{
    private void WalkToPositionOnUpdate()
    {
        // 1. set momentum as a function of distance from target (slow down at end) and maybe also time in state (speed up at start)
        HandleTurning(1f, false, 0f); // 2. abstract away HandleTurningCore to support turning based on non-input vectors
        // 3. collisionmove on transform

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void WalkToPositionConfigure()
    {
        Machine.Configure(PlayerFsmState.WalkToPosition)
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