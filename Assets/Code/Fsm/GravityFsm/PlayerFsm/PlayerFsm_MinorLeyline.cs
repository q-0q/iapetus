using UnityEngine;

public partial class PlayerFsm
{
    private void MinorLeylineConfigure()
    {
        Machine.Configure(PlayerFsmState.MinorLeylineInteractable)
            .Permit(PlayerFsmTrigger.MinorLeylineTrigger, PlayerFsmState.MinorLeylineStartup);
        
        Machine.Configure(PlayerFsmState.MinorLeylineStartup)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.MinorLeylineActive);
        
        Machine.Configure(PlayerFsmState.MinorLeylineActive)
            .Permit(PlayerFsmTrigger.MinorLeylineTrigger, PlayerFsmState.GroundMove);
    }
    
    private void MinorLeylineStartupOnUpdate()
    {
        
    }
    
    private void MinorLeylineActiveOnUpdate()
    {
        
    }
}