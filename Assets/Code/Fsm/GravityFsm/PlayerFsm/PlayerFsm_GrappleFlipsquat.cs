public partial class PlayerFsm
{
    private void GrappleFlipsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.GrappleFlipsquat)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GrappleFlip)
            .OnEntry(_ =>
            {
                // transform.DOShakePosition(0.5f, 0.3f);
                // ReplaceAnimatorTrigger("GrappleFlipsquat");
                // HitstopManager.Singleton.StartHitstop(0.075f);
            })
            .OnExit(_ => { });
    }
}