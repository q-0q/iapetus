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
        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);

        var targetMomentum = _stateEntryMomentum < ImpaleMinimumMomentumAfterOffset
            ? _momentum
            : Mathf.Max(_stateEntryMomentum + ImpaleMomentumOffset, ImpaleMinimumMomentumAfterOffset);
        _momentum = Mathf.Lerp(_momentum, targetMomentum, Time.deltaTime * ImpaleMomentumLerpStrenth);
    }

    private void ImpaleGroundConfigure()
    {
        Machine.Configure(PlayerFsmState.ImpaleGround)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Attack");
                OnPlayerImpaleStateEntered?.Invoke();
                Animator.SetTrigger("Impale");
                _stateEntryMomentum = _momentum;
            }).OnExit(_ => { Animator.ResetTrigger("Impale"); });
    }
}