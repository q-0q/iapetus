using UnityEngine;

public partial class PlayerFsm
{

    private void SkipOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        transform.position += ComputeCollisionMove(transform.forward * (Time.deltaTime * SkipForwardBonusSpeed));
    }
    
    private void SkipConfigure()
    {
        Machine.Configure(PlayerFsmState.Skip)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .PermitIf(PlayerFsmTrigger.StartUpdraft, PlayerFsmState.Updraft, _ => TimeInCurrentState() > 0.35f)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.LandsquatAfterDash, _ => true, 1)
            .OnEntry(_ =>
            {
                _momentum = 14f;
                IncrementCombo();
                _inputBuffer.ConsumeBuffer("Jump");
                FMODUnity.RuntimeManager.PlayOneShotAttached(skipFmodEvent, gameObject);
            })
            .OnEntryFrom(FsmTrigger.Timeout, _ => { YVelocity = SkipYVelocity; });
    }
}