using UnityEngine;

public partial class PlayerFsm
{
    private void SurgeStartupOnUpdate()
    {

    }
    
    private void SurgeDashOnUpdate()
    {
        var movementMofifier = Mathf.Lerp(2f, 1f, Mathf.InverseLerp(0, 0.1f, TimeInCurrentState()));
        HandleCollisionMove(movementMofifier);
    }
    
    
    private void SurgeConfigure()
    {
        Machine.Configure(PlayerFsmState.SurgeStartup)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SurgeDash)
            .OnExit(_ =>
            {
                StartSurge();
            })
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
            });
        
        Machine.Configure(PlayerFsmState.SurgeDash)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnExit(_ =>
            {
            })
            .OnEntry(_ =>
            {
            });
    }
}