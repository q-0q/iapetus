public partial class PlayerFsm
{
    private void LandableConfigure()
    {
        Machine.Configure(PlayerFsmState.Landable)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => CurrentFallDistance() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => CurrentFallDistance() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2);
    }
}