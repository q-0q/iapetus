using UnityEngine;

public partial class PlayerFsm
{
    private void ImpaleAirOnUpdate()
    {
        Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 1, Time.deltaTime * 90f));
        Animator.SetLayerWeight(1, 0);

        var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);

        var targetMomentum = _stateEntryMomentum < ImpaleMinimumMomentumAfterOffset
            ? _momentum
            : Mathf.Max(_stateEntryMomentum + ImpaleMomentumOffset, ImpaleMinimumMomentumAfterOffset);
        _momentum = Mathf.Lerp(_momentum, targetMomentum, Time.deltaTime * ImpaleMomentumLerpStrenth);
    }

    private void ImpaleAirConfigure()
    {
        Machine.Configure(PlayerFsmState.ImpaleAir)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(PlayerFsmState.Landable)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Attack");
                OnPlayerImpaleStateEntered?.Invoke();
                Animator.SetTrigger("ImpaleJump");
                _stateEntryMomentum = _momentum;
            }).OnExit(_ => { Animator.ResetTrigger("ImpaleJump"); });
    }
}