using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{

    private void RopeSwingOnUpdate()
    {
        // if (_playerInput.actions["Sprint"].IsPressed()) isSprinting = true;
        
        // HandleInputMomentumChange();
        HandleTurning(0f, false, 1f, false, isSprinting ? 0.5f : 1f);
        
        // SetAnimatorMomentum();
        // SetAnimatorSpeedMod();

        var desiredPosition = _currentRopeSwing.GetWorldspaceAttachPoint();
        transform.position = Vector3.Lerp(transform.position, desiredPosition,
            Time.deltaTime * Mathf.Lerp(8f, 40f, Mathf.InverseLerp(0, 0.5f, TimeInCurrentState())));
    }
    
    private void RopeSwingConfigure()
    {

        Machine.Configure(PlayerFsmState.RopeSwingInteractable)
            .PermitIf(PlayerFsmTrigger.EnterRopeSwingTrigger, PlayerFsmState.RopeSwing, @params =>
            {
                if (Machine.IsInState(PlayerFsmState.Jump) && TimeInCurrentState() < 0.1f) return false;
                return YVelocity < 15f;
            });
        
        
        Machine.Configure(PlayerFsmState.RopeSwing)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.DontLoseYVelocity)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInCurrentState() > 0)
            .OnEntry(_ =>
            {
                // _momentum = 0;
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
            })
            .OnEntryFrom(PlayerFsmTrigger.EnterRopeSwingTrigger, @params =>
            {
                if (@params is not RopeSwingHitParam ropeSwingHitParam) return;
                _currentRopeSwing = ropeSwingHitParam.RopeSwing;
                _currentRopeSwing.SetPlayerAttachment(transform.position, _momentum);
            });
        
    }
    
}