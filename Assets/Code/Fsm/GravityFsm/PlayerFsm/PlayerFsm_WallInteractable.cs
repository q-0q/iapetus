using System;
using Wasp;

public partial class PlayerFsm
{
    private void WallInteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, CanVault, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => !Machine.IsInState(PlayerFsmState.PitonFlip) || YVelocity < PitonMaximumWallInteractYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && WallsquatVelocityChecker() && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.MediumVaultHang,
                IsTightropeTrigger, 1)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && WallsquatVelocityChecker() && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && WallsquatVelocityChecker() && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => WallrunVelocityChecker());
    }

    private bool CanVault(TriggerParams t)
    {
        return (YVelocity > VaultMinimumYVelocity && _momentum > VaultMinimumMomentum);
    }

    private bool WallsquatVelocityChecker()
    {
        if (Machine.IsInState(PlayerFsmState.PitonFlip)) return YVelocity < PitonMaximumWallInteractYVelocity;
        return YVelocity < WallsquatMinimumYVelocity || IsInGust;
    }
    
    private bool WallrunVelocityChecker()
    {
        return isSprinting && _momentum > WallRunMinimumMomentum && (YVelocity < WallRunMinimumYVelocity || IsInGust);
    }
}