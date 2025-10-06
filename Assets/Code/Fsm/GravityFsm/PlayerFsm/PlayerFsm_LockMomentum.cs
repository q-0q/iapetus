using UnityEngine;

public partial class PlayerFsm
{
    private void LockMomentumOnUpdate()
    {
        Animator.SetFloat("SpeedMod", Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
        SetAnimatorMomentum();
        HandleCollisionMove();
    }
}