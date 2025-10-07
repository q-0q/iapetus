using UnityEngine;

public partial class PlayerFsm
{
    private void VaultOnUpdate()
    {
        _momentum = Mathf.Max(_momentum, VaultMinimumMomentum);
        var momentumWeight = ComputeMomentumWeight();
        Animator.SetFloat("SpeedMod",
            Mathf.Lerp(VaultMinimumAnimatorSpeedMod, VaultMaximumAnimatorSpeedMod, momentumWeight));
        MoveYOntoLedge(0f, VaultLedgeLerpStrength);
        SetAnimatorMomentum();
        transform.position += ComputeCollisionMove(ComputeDesiredMove());
        HandleTurning(VaultTurningMultiplier, true);
    }

    private void VaultConfigure()
    {
        Machine.Configure(PlayerFsmState.Vault)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            // .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .OnEntry(_ =>
            {
                _movementAnimationMirror = !_movementAnimationMirror;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
                UpdateLedgePosition(FaceLedgeHeight);
                ReplaceAnimatorTrigger("Vault");
                YVelocity = 0;
            });
    }
}