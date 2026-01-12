public partial class PlayerFsm
{
    private void UpdraftOnUpdate()
    {
        if (TimeInCurrentState() > 0.25f)
        {
            _wallsquattedSinceLeavingGround = false;
            _dashSinceLeavingGround = false;
            _previousWallrunSide = FlankType.None;
            _currentFlankType = FlankType.None;
            _wallsquattedSinceLeavingGround = false;
        }
    }
    private void UpdraftConfigure()
    {
        Machine.Configure(PlayerFsmState.Updraft)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(PlayerFsmState.Landable)
            .Permit(PlayerFsmTrigger.EndUpdraft, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            .OnEntry(_ =>
            {
                // _movementAnimationMirror = !_movementAnimationMirror;
                // var flip = _movementAnimationMirror ? 0 : 1f;
                // Animator.SetFloat("Flip", flip);
            });
    }
}