using UnityEngine;

public partial class PlayerFsm
{
    private void HardLandOnUpdate()
    {
        if (TimeInCurrentState() < HardLandForwardDuration)
        {
            transform.position += ComputeCollisionMove(transform.forward * HardLandForwardSpeed * Time.deltaTime);
        }
    }
    private void HardLandConfigure()
    {
        Machine.Configure(PlayerFsmState.HardLand)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.StandardGroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandExitMomentum;
                ReplaceAnimatorTrigger("HardLand");
            });
    }
}