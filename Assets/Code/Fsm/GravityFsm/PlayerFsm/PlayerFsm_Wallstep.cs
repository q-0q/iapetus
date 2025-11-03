using UnityEngine;

public partial class PlayerFsm
{
    private void WallstepConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallstep)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.SlowVaultHang,
                _ => YVelocity < MediumVaultHangMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Jump");
                YVelocity = Mathf.Lerp(WallstepMinimumYVelocityGain, WallstepMaximumYVelocityGain,
                    ComputeMomentumWeight());
                Animator.SetFloat("VerticalMomentum", ComputeMomentumWeight());
                _momentum = 0;
            });
    }
}