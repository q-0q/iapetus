using Unity.Mathematics;
using UnityEngine;

public partial class PlayerFsm
{
    private void HardLandRollOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        transform.position += ComputeCollisionMove(transform.forward * (HardLandRollForwardSpeed * Time.deltaTime));
        HandleTurning(AirControlTurningMultiplier, true, 0, true);
    }

    private void HardLandRollConfigure()
    {
        Machine.Configure(PlayerFsmState.HardLandRoll)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(impactFmodEvent, gameObject);
                OnPlayerFootstep();
                LastUpwardsY = transform.position.y;
                _momentum = HardLandRollExitMomentum;
            });
    }
}