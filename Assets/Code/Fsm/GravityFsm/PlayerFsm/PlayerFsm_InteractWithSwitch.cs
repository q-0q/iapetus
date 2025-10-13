using UnityEngine;

public partial class PlayerFsm
{
    private void InteractWithSwitchOnUpdate()
    {

    }
    private void InteractWithSwitchConfigure()
    {
        Machine.Configure(PlayerFsmState.InteractWithSwitch)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("InteractWithSwitch");
            });
    }
}