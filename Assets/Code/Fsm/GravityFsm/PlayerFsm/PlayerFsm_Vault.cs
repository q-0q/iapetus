using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public partial class PlayerFsm
{
    private void VaultOnUpdate()
    {
        _momentum = Mathf.Max(_momentum, VaultMinimumMomentumOnUpdate);
        var momentumWeight = ComputeMomentumWeight();
        Animator.SetFloat("SpeedMod",
            Mathf.Lerp(VaultMinimumAnimatorSpeedMod, VaultMaximumAnimatorSpeedMod, momentumWeight));
        if (UpdateLedgePosition(FaceLedgeHeight))
        {
            MoveYOntoLedge(0f, VaultLedgeLerpStrength);
        }
        SetAnimatorMomentum();
        var movementModifier = Machine.IsInState(PlayerFsmState.DashVault) ? 0.3f : 0.9f;
        transform.position += ComputeCollisionMove(ComputeDesiredMove()) * movementModifier;
        HandleTurning(VaultTurningMultiplier, true);
    }

    private void VaultConfigure()
    {
        Machine.Configure(PlayerFsmState.Vault)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            // .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(FsmTrigger.Timeout, PlayerFsmState.Jumpsquat, _ => _inputBuffer.IsBuffered("Jump"), 1)
            .OnEntry(_ =>
            {
                _movementAnimationMirror = !_movementAnimationMirror;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
                UpdateLedgePosition(FaceLedgeHeight);
                YVelocity = 0;
                
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                currentRopeSwing = null;
                
                FMODUnity.RuntimeManager.PlayOneShotAttached(jumpFmodEvent, gameObject);
                OnPlayerFootstep();
                
                IncrementCombo();
            })
            .OnExit(_ =>
            {
                _momentum = Mathf.Min(MaxMomentum, _momentum + 2f);
            });
        
        Machine.Configure(PlayerFsmState.DashVault)
            .PermitIf(FsmTrigger.Timeout, PlayerFsmState.Skip, _ => _inputBuffer.IsBuffered("Jump", 0.25f), 2)
            .SubstateOf(PlayerFsmState.Vault);
    }
    
    
}