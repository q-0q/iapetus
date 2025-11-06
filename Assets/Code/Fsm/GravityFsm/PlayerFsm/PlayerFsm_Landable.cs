using Code.TriggerParams;
using Wasp;

public partial class PlayerFsm
{
    private void LandableConfigure()
    {
        Machine.Configure(PlayerFsmState.Landable)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Skipsquat,
                _ => Machine.IsInState(PlayerFsmState.FallAfterDash) && _inputBuffer.IsBuffered("Jump"),
                3) // ANTI PATTERN
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand,
                _ => CurrentFallDistance() < HardLandAirDiff,
                2)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => CurrentFallDistance() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 4)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Slide, _ => _slopeTimer > 0.2f, 5);

    }
    
    private bool IsTightropeTrigger(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        return raycastHitParam.kind == GroundKind.Tightrope;
    }
}