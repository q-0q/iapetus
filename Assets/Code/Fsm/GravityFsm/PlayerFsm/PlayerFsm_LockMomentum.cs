using UnityEngine;

public partial class PlayerFsm
{
    private void LockMomentumOnUpdate()
    {
        Animator.SetFloat("SpeedMod", Mathf.Lerp(0, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
        SetAnimatorMomentum();
        HandleCollisionMove();
    }
}