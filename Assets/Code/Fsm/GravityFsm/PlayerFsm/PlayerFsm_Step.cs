
using UnityEngine;

public partial class PlayerFsm
{
    private void IdleOnUpdate()
    {
        HandleInputMomentumChange();
        transform.position += ComputeCollisionMove(ApplyTraction(Vector3.zero));
    }
    
    private void StepStartOnUpdate()
    {
        HandleInputMomentumChange();
        HandleTurning(2f);

        var movement = transform.forward * (3f * Time.deltaTime);
        transform.position += ComputeCollisionMove(ApplyTraction(movement));
        SetAnimatorMomentum();
    }
    
    private void StepEndOnUpdate()
    {
        HandleInputMomentumChange();
        transform.position += ApplyTraction(Vector3.zero);
    }
    
    private void StepConfigure()
    {

        
        Machine.Configure(PlayerFsmState.Idle)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(PlayerFsm.PlayerFsmTrigger.Accelerating, PlayerFsm.PlayerFsmState.StepStart)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat,
                _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.TightropeMove, IsTightropeTrigger, 6);
        
        Machine.Configure(PlayerFsmState.StepStart)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat,
                _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.TightropeMove, IsTightropeTrigger, 6)
            .OnExit(_ =>
            {
                _momentum = Mathf.Max(_momentum, 3f);
            });
        
        Machine.Configure(PlayerFsmState.StepEnd)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat,
                _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.TightropeMove, IsTightropeTrigger, 6)
            .OnEntry(_ =>
            {
                _previousPositionDelta *= 0.35f;
                _momentum = 0;
            });
        
    }
}