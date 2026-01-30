public partial class PlayerFsm
{
    private void LandsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Landsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                FMODUnity.RuntimeManager.PlayOneShotAttached(landFmodEvent, gameObject);
                OnPlayerFootstep();
                print("Landsquat OnEntry");
            })
            .OnExit(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                
                _movementAnimationMirror = !_movementAnimationMirror;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
                
            });

        Machine.Configure(PlayerFsmState.LandsquatAfterDash)
            .SubstateOf(PlayerFsmState.Landsquat);
        
        // Machine.Configure(PlayerFsmState.LandsquatTightrope)
        //     .PermitIf(FsmTrigger.Timeout, PlayerFsmState.TightropeMove, _=> true, 1)
        //     .SubstateOf(PlayerFsmState.Landsquat);
        
    }
}