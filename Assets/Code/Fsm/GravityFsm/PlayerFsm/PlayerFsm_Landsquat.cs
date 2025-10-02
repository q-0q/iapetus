public partial class PlayerFsm
{
    private void LandsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Landsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Landsquat"); })
            .OnExit(_ =>
            {
                _movementAnimationMirror = !_movementAnimationMirror;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
            });
    }
}