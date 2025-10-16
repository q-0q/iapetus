public partial class PlayerFsm
{
    private void GrappleFlipConfigure()
    {
        Machine.Configure(PlayerFsmState.GrappleFlip)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .OnEntry(_ =>
            {
                _momentum = MaxMomentum;
                YVelocity = 10;
                // ReplaceAnimatorTrigger("GrappleFlip");
            });
    }
}