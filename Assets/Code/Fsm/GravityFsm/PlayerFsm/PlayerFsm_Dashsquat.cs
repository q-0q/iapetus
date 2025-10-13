public partial class PlayerFsm
{
    private void DashsquatOnUpdate()
    {
        DoGenericCollisionMove();
        HandleTurning(DashsquatTurnMultiplier, true);
    }

    private void DashsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Dashsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Grapple)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Dash");
                ReplaceAnimatorTrigger("Dashsquat");
            });
    }
}