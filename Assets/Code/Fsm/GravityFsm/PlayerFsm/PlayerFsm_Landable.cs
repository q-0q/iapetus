public partial class PlayerFsm
{
    private void LandableConfigure()
    {
        Machine.Configure(PlayerFsmState.Landable)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2);
    }
}