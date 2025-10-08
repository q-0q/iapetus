public partial class PlayerFsm
{
    private void WallInteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity && _momentum > VaultMinimumMomentum, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity && !_wallsquattedSinceLeavingGround)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity);
    }
}