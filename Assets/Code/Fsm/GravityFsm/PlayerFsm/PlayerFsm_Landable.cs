using Code.TriggerParams;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{
    private void LandableConfigure()
    {
        Machine.Configure(PlayerFsmState.Landable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat, @params =>
            {
                if (WaterRaycast(out var hit))
                {
                    if (IsSwimTrigger(new RaycastHitParam() { Hit = hit })) return false;
                }
                return !IsSlideTrigger(@params);
            })
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Skipsquat,
                _ => Machine.IsInState(PlayerFsmState.FallAfterDash) && _inputBuffer.IsBuffered("Jump"),
                3) // ANTI PATTERN
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand,
                _ => CurrentFallDistance() < HardLandAirDiff,
                2)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => (CurrentFallDistance() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum), 4)
        // .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Slide, _ => _slopeTimer > 0.2f, 5)
            .SubstateOf(PlayerFsmState.SlideInteractable)
            .OnExitFrom(GravityFsmTrigger.StartFrameGrounded, @params =>
            {
                if (@params is not RaycastHitParam param) return;
                // print("startframegrounded: " + param.Hit.collider.name );
            })
            .PermitIf(PlayerFsmTrigger.SwimTriggerRaycastHit, PlayerFsmState.SwimSurfaceRise, IsSwimTrigger);

    }
    

    
    private bool IsTightropeTrigger(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        return raycastHitParam.kind == GroundKind.Tightrope;
    }
}