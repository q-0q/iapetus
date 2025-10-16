using UnityEngine;

public partial class PlayerFsm
{

    private void SkipOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        transform.position += ComputeCollisionMove(transform.forward * (Time.deltaTime * SkipForwardBonusSpeed));
    }
    
    private void SkipConfigure()
    {
        Machine.Configure(PlayerFsmState.Skip)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1);
    }
}