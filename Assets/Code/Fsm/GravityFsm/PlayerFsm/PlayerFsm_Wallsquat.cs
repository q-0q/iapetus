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
            .PermitIf(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall, _ => TimeInCurrentState() > 0.2f)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep,
                _ => TimeInCurrentState() > WallstepMinimumDuration, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                LastUpwardsY = transform.position.y;
                _wallsquattedSinceLeavingGround = true;
            })
            .OnExit(_ =>
            {
                YVelocity = 0;
            })
            .OnExitFrom(PlayerFsmTrigger.FaceOpen, _ => { _momentum = 0; });
    }
}