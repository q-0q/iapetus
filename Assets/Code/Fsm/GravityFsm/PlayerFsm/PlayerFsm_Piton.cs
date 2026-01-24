using UnityEngine;
using Wasp;

public partial class PlayerFsm
{

    private void PitonsquatOnUpdate()
    {
        transform.position = _currentPitonTransform.position;
    }

    private void PitonConfigure()
    {

        Machine.Configure(PlayerFsmState.PitonHoming)
            .Permit(PlayerFsmTrigger.ArriveAtPiton, PlayerFsmState.Pitonsquat);

        Machine.Configure(PlayerFsmState.Pitonsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.PitonFlipsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall);

        Machine.Configure(PlayerFsmState.PitonFlipsquat)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.PitonFlip);
        
        Machine.Configure(PlayerFsmState.PitonFlip)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            // .SubstateOf(PlayerFsmState.WallInteractable)
            .OnEntry(_ =>
            {
                _momentum = MaxMomentum;
                YVelocity = 10;
            });
    }
}