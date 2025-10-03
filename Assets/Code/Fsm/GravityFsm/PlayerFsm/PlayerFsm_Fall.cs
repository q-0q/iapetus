public partial class PlayerFsm
{
    private void FallConfigure()
    {
        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Fall"); });
    }
}