public partial class PlayerFsm
{
    private void FallAfterDashConfigure()
    {
        Machine.Configure(PlayerFsmState.FallAfterDash)
            .SubstateOf(PlayerFsmState.Fall);
    }
}