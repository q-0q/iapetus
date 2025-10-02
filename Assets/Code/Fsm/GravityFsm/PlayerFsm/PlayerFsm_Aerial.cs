public partial class PlayerFsm
{
    private void AerialOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
    }

    private void AerialConfigure()
    {
        Machine.Configure(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.LockMomentum);
    }
}