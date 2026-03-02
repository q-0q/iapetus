public partial class PlayerFsm
{
    private void FallConfigure()
    {
        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .Permit(PlayerFsmTrigger.StartUpdraft, PlayerFsmState.Updraft)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, @params => CanDash(@params) && TimeInCurrentState() > 0.1f); // microfall dash prevention hack.

    }
}