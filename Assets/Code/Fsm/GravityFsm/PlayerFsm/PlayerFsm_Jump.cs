public partial class PlayerFsm
{
    private void JumpConfigure()
    {
        Machine.Configure(PlayerFsmState.Jump)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Jump"); });
    }
}