using UnityEngine;

public partial class PlayerFsm
{
    private void SurgeStartupOnUpdate()
    {

    }
    private void SurgeConfigure()
    {
        Machine.Configure(PlayerFsmState.SurgeStartup)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnExit(_ =>
            {
                StartSurge();
            })
            .OnEntry(_ =>
            {
                
            });
    }
}