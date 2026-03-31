using UnityEngine;

public partial class PlayerFsm
{
    private void LockMomentumOnUpdate()
    {
        if (Machine.IsInState(PlayerFsmState.Dash)) return;
        if (Machine.IsInState(PlayerFsmState.SurgeDash)) return;
        Animator.SetFloat("SpeedMod", Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
        SetAnimatorMomentum();
        HandleCollisionMove();
    }
}