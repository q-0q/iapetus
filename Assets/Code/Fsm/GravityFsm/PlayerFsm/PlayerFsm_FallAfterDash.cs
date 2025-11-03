public partial class PlayerFsm
{

    private void FallAfterDashOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
    }
    
    private void FallAfterDashConfigure()
    {
        Machine.Configure(PlayerFsmState.FallAfterDash)
            .SubstateOf(PlayerFsmState.Fall)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.LandsquatAfterDash, _ => true, 1);
    }
}