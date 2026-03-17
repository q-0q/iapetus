using System;

public partial class PlayerFsm
{

    public static event Action OnPlayerEnterUpdraft;
    private void UpdraftOnUpdate()
    {
        if (TimeInCurrentState() > 0.45f)
        {
            _dashSinceLeavingGround = false;
            _wallsquattedSinceLeavingGround = false;
        }
        Animator.SetFloat("UpdraftAmount", YVelocity);
    }
    private void UpdraftConfigure()
    {
        Machine.Configure(PlayerFsmState.Updraft)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.DontLoseYVelocity)
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
                OnPlayerEnterUpdraft?.Invoke();
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                currentRopeSwing = null;
            });
    }
}