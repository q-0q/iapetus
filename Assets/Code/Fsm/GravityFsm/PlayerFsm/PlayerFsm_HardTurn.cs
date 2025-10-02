using UnityEngine;

public partial class PlayerFsm
{
    private void HardTurnOnUpdate()
    {
        _momentum = Mathf.Max(0, _momentum - MomentumLossRate * Time.deltaTime * HardTurnMomentumLossModifier);
        Animator.SetLayerWeight(2, 0);
    }

    private void HardTurnConfigure()
    {
        Machine.Configure(PlayerFsmState.HardTurn)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .Permit(PlayerFsmTrigger.NoMomentum, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ => { ReplaceAnimatorTrigger("HardTurn"); });
    }
    
}