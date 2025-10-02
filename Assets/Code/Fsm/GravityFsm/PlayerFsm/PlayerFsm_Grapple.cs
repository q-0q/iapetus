using UnityEngine;

public partial class PlayerFsm
{
    private void GrappleOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        var collisionMove = ComputeCollisionMove(transform.forward * (DashForwardSpeed * Time.deltaTime));
        transform.position += collisionMove;
    }

    private void GrappleConfigure()
    {
        Machine.Configure(PlayerFsmState.Grapple)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GrappleFlipsquat)
            // .Permit(PlayerFsmTrigger.ContactHitboxTrigger, PlayerFsmState.GrappleFlipsquat)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat, _ => true)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                _momentum = Mathf.Min(Mathf.Max(_momentum + DashEntryMomentumGain, DashEntryMinimumMomentum),
                    MaxMomentum);
                transform.rotation =
                    Quaternion.LookRotation(PlayerWeaponFsm.Singleton.transform.position - transform.position,
                        Vector3.up);
                ReplaceAnimatorTrigger("Dash");
            })
            .OnExit(_ =>
            {
                transform.position = PlayerWeaponFsm.Singleton.transform.position - transform.forward * 0.75f;
            });
    }
}