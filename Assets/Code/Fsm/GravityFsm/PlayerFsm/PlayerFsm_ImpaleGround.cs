using UnityEngine;

public partial class PlayerFsm
{
    private void ImpaleGroundOnUpdate()
    {
        Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 1, Time.deltaTime * 90f));
        Animator.SetLayerWeight(1, 0);
        HandleInputMomentumChange();

        HandleTurning(0.75f, true);
        HandleCollisionMove(ImpaleMovementModifier);

        SetAnimatorMomentum();
        var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);

        var targetMomentum = _stateEntryMomentum < ImpaleMinimumMomentumAfterOffset
            ? _momentum
            : Mathf.Max(_stateEntryMomentum + ImpaleMomentumOffset, ImpaleMinimumMomentumAfterOffset);
        _momentum = Mathf.Lerp(_momentum, targetMomentum, Time.deltaTime * ImpaleMomentumLerpStrenth);
    }
}