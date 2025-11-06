using UnityEngine;

public partial class PlayerFsm
{
    private void HardLandOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        if (TimeInCurrentState() < HardLandForwardDuration)
        {
            transform.position += ComputeCollisionMove(transform.forward * (HardLandForwardSpeed * Time.deltaTime));
        }
    }
    private void HardLandConfigure()
    {
        Machine.Configure(PlayerFsmState.HardLand)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandExitMomentum;
            });
    }
}