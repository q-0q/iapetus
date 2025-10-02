public partial class PlayerFsm
{
    private void HardLandConfigure()
    {
        Machine.Configure(PlayerFsmState.HardLand)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandExitMomentum;
                ReplaceAnimatorTrigger("HardLand");
            });
    }
}