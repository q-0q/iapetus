using UnityEngine;

public partial class PlayerFsm
{
    private void DashOnUpdate()
    {
        HandleTurning(AirControlTurningMultiplier, true, AirControlTurningMomentumDecayModifier);
        Animator.SetLayerWeight(1, 0);
        var collisionMove = ComputeCollisionMove(transform.forward * (DashForwardSpeed * Time.deltaTime));
        transform.position += collisionMove;
    }

    private void DashConfigure()
    {
        Machine.Configure(PlayerFsmState.Dash)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.FallAfterDash)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Skipsquat)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                YVelocity = Mathf.Max(YVelocity, 12f);
            })
            .OnExit(_ =>
            {
                _momentum = 13f;
            });
    }
}