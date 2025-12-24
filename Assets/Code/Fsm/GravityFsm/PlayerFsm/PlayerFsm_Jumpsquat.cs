public partial class PlayerFsm
{
    private void JumpsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Jumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Jump");
            })
            .OnExitFrom(FsmTrigger.Timeout, _ => { YVelocity = JumpYVelocity; });
        
    }
}