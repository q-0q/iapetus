public partial class PlayerFsm
{
    private void TightropeLandsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.TightropeJumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .SubstateOf(PlayerFsmState.Tightrope)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.TightropeGroundMove)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Landsquat"); })
            .OnExit(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                _movementAnimationMirror = !_movementAnimationMirror;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
            });
    }
}