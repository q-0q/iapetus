using UnityEngine;

public partial class PlayerFsm
{
    private void LockMomentumOnUpdate()
    {
        if (Machine.IsInState(PlayerFsmState.Dash)) return; // ANTI PATTERN WE NEED TO FIX THIS
        Animator.SetFloat("SpeedMod", Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
        SetAnimatorMomentum();
        HandleCollisionMove();
    }
}