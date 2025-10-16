public partial class PlayerFsm
{
    private void SkipsquatOnUpdate()
    {
        HandleCollisionMove(0.25f);
    }
    
    private void SkipsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Skipsquat)
            .SubstateOf(GravityFsmState.Grounded)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Skip)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
            })
            .OnExitFrom(FsmTrigger.Timeout, _ => { YVelocity = JumpYVelocity; });
    }
}