using UnityEngine;

public partial class PlayerFsm
{
    private void LockMomentumOnUpdate()
    {
        if (Machine.IsInState(PlayerFsmState.Dash)) return;
        if (Machine.IsInState(PlayerFsmState.SurgeDash)) return;
        Animator.SetFloat("SpeedMod", Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
        SetAnimatorMomentum();


        if (!Machine.IsInState(PlayerFsmState.Wallrun) && !Machine.IsInState(PlayerFsmState.HardTurn) && !Machine.IsInState(PlayerFsmState.Slide) && !Machine.IsInState(PlayerFsmState.PitonHoming))
        {
            HandleInputMomentumChange();
            HandleTurning(1f, false, 1f, false, isSprinting ? 0.5f : 1f);
        }
        
        HandleCollisionMove();
    }
}