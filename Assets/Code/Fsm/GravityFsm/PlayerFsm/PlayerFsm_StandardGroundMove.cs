using UnityEngine;

public partial class PlayerFsm
{
    private void StandardGroundMoveOnUpdate()
    {
        HandleInputMomentumChange();
        HandleTurning();
        DoGenericCollisionMove();

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void StandardGroundMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.StandardGroundMove)
            .SubstateOf(PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("GroundMove");
            });
    }
}