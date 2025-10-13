using UnityEngine;

public partial class PlayerFsm
{
    private void WalkToPositionOnUpdate()
    {
        var isInTurnPhase = TimeInCurrentState() < WalkToPositionTurnPhaseDuration;
        var toTarget = _walkToPositionTarget - transform.position;
        toTarget = new Vector3(toTarget.x, 0, toTarget.z);
        _momentum = Mathf.Lerp(_momentum, isInTurnPhase ? 0 : WalkToPositionMomentum, Time.deltaTime * WalkToPositionMomentumLerpStrength);
        HandleTurningCore(1f, 0f, toTarget);
        HandleCollisionMove();
        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void WalkToPositionConfigure()
    {
        Machine.Configure(PlayerFsmState.WalkToPosition)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                ReplaceAnimatorTrigger("GroundMove");
            });
    }
}