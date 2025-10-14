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
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            // .Permit(PlayerFsmTrigger.ContactHitboxTrigger, PlayerFsmState.DashFlipsquat)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat, _ => true)
            // .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                // var transformPosition = new Vector3(PlayerWeaponFsm.Singleton.transform.position.x, transform.position.y,
                //     PlayerWeaponFsm.Singleton.transform.position.z);
                // var forward = transformPosition - transform.position;
                // transform.rotation =
                //     Quaternion.LookRotation(forward,
                //         Vector3.up);
                YVelocity = Mathf.Max(YVelocity, 15f);
                ReplaceAnimatorTrigger("Dash");
            })
            .OnExit(_ =>
            {
                _momentum = 13f;
            });
    }
}