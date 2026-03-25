public partial class PlayerFsm
{
    private void SkipsquatOnUpdate()
    {
        HandleCollisionMove(0.25f, false);
    }
    
    private void SkipsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Skipsquat)
            .SubstateOf(GravityFsmState.Grounded)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Skip)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(dashWhooshEventReference, gameObject);
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                currentRopeSwing = null;
                OnPlayerFootstep();
            });
    }
}