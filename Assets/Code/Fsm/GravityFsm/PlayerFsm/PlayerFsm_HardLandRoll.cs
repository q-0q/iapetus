using UnityEngine;

public partial class PlayerFsm
{
    private void HardLandRollOnUpdate()
    {
        transform.position += ComputeCollisionMove(transform.forward * (HardLandRollForwardSpeed * Time.deltaTime));
        HandleTurning(AirControlTurningMultiplier, true, 0);
    }

    private void HardLandRollConfigure()
    {
        Machine.Configure(PlayerFsmState.HardLandRoll)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandRollExitMomentum;
                ReplaceAnimatorTrigger("HardLandRoll");
            });
    }
}