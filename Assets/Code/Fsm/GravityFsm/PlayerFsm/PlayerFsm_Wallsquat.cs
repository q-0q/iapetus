public partial class PlayerFsm
{
    private void WallsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            // .Permit(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep,
                _ => TimeInCurrentState() > WallstepMinimumDuration, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = true;
                YVelocity = 0;
                ReplaceAnimatorTrigger("Wallsquat");
            })
            .OnExitFrom(PlayerFsmTrigger.FaceOpen, _ => { _momentum = 0; });
    }
}