public partial class PlayerFsm
{
    private void DashsquatOnUpdate()
    {
        // HandleCollisionMove(0.5f);
        // HandleTurning(DashsquatTurnMultiplier, true);
    }

    private void DashsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Dashsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Dash)
            // .SubstateOf(GravityFsmState.DontApplyYVelocity)
            // .SubstateOf(PlayerFsmState.LockMomentum)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Dash");
                ReplaceAnimatorTrigger("Dashsquat");
            });
    }
}