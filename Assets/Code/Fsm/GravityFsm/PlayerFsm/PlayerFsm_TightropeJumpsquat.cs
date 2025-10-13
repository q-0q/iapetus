public partial class PlayerFsm
{
    private void TightropeJumpsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.TightropeJumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .SubstateOf(PlayerFsmState.Tightrope)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jumpsquat");
                _inputBuffer.ConsumeBuffer("Jump");
            })
            .OnExitFrom(FsmTrigger.Timeout, _ => { YVelocity = JumpYVelocity; });
    }
}