using UnityEngine;

public partial class PlayerFsm
{
    private void VaultOnUpdate()
    {
        _momentum = Mathf.Max(_momentum, VaultMinimumMomentumOnUpdate);
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
            .SubstateOf(PlayerFsmState.IgnoreFailsafe)
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
            })
            .OnExit(_ =>
            {
                _momentum = Mathf.Min(MaxMomentum, _momentum + 2f);
            });
    }
}