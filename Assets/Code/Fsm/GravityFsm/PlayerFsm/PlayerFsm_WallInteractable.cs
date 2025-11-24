using System;
using Wasp;

public partial class PlayerFsm
{
    private void WallInteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, CanVault, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.MediumVaultHang,
                IsTightropeTrigger, 1)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity && _playerInput.actions["Jump"].IsPressed());
    }

    private bool CanVault(TriggerParams t)
    {
        return (YVelocity > VaultMinimumYVelocity && _momentum > VaultMinimumMomentum);
    }
}