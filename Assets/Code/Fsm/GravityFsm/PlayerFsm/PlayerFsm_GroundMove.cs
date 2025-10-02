using UnityEngine;

public partial class PlayerFsm
{
    private void GroundMoveOnUpdate()
    {
        HandleInputMomentumChange();
        HandleTurning();
        HandleCollisionMove();

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }
}