using UnityEngine;

public partial class PlayerFsm
{
    private void WallrunOnUpdate()
    {
        SetAnimatorMomentum();
        HandleFlankAlignment();
        HandleCollisionMove();

        transform.position +=
            ComputeCollisionMove(-_currentFlankWallNormal * (Time.deltaTime * FlankWallVacuumStrength));
    }

    private void WallrunConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallrun)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.FlankOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .OnEntry(_ =>
            {
                print("Yvelocity on entering wallrun: " + YVelocity);
                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                ReplaceAnimatorTrigger("Wallrun");
            })
            .OnExitFrom(PlayerFsmTrigger.Jump, _ =>
            {
                var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
                var forward = Quaternion.Euler(0f, WallrunJumpAngle * rotationMod, 0f) * _currentFlankWallNormal;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            })
            .OnExit(_ => { _previousWallrunSide = _currentFlankType; });
    }
}