using UnityEngine;

public partial class PlayerFsm
{
    private void WallstepConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallstep)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat, @params => !IsSlideTrigger(@params))
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.SlowVaultHang,
                _ => YVelocity < MediumVaultHangMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Jump");
                if (IsInGust)
                {
                    YVelocity = WallstepMaximumYVelocityGain;
                    Animator.SetFloat("VerticalMomentum", 1f);
                }
                else
                {
                    YVelocity = Mathf.Lerp(WallstepMinimumYVelocityGain, WallstepMaximumYVelocityGain,
                        ComputeMomentumWeight());
                    Animator.SetFloat("VerticalMomentum", ComputeMomentumWeight());
                }
                _momentum = 0;
                
                OnPlayerFootstep();
                FMODUnity.RuntimeManager.PlayOneShotAttached(jumpFmodEvent, gameObject);
            });
    }
}