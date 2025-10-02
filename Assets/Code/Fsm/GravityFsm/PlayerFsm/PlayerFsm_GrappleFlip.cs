public partial class PlayerFsm
{
    private void GrappleFlipConfigure()
    {
        Machine.Configure(PlayerFsmState.GrappleFlip)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity)
            .OnEntry(_ =>
            {
                _momentum = 10f;
                ReplaceAnimatorTrigger("GrappleFlip");
                YVelocity = 30;
            });
    }
}