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
            .Permit(PlayerFsmTrigger.EndUpdraft, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash);
    }
}