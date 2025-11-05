using Cinemachine;
using UnityEngine;

public partial class PlayerFsm
{
    private void TightropeMoveOnUpdate()
    {
        var tightropeStart = _springCollider.tightropeController.transform;
        var tightropeEnd = _springCollider.tightropeController.end;
        var v3 = GetInputMovementVector3();
        
        if (v3.magnitude < InputMagnitudeThreshhold) v3 = transform.forward;
        var angle = Vector3.Angle(v3, tightropeStart.position - transform.position);
        var target = angle < 90f ? tightropeStart : tightropeEnd;
        var targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 20f);



        HandleInputMomentumChange();
        SetAnimatorMomentum();

        var collisionMove = ComputeCollisionMove((target.position - _springCollider.transform.position).normalized * ComputeDesiredMove().magnitude);
        
        _springCollider.transform.position += collisionMove;
        

        var speedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
        Animator.SetFloat("SpeedMod", speedMod);
    }

    private void TightropeMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.TightropeMove)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            // .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            // .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            // .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            // .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            // .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Slide, _ => _slopeTimer > 0.2f)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
            });
    }
}