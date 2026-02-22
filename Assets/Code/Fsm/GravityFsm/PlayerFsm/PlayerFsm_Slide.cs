using Code.Misc;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using Wasp;

public partial class PlayerFsm
{
    private void SlideLateralOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        GetGroundedRaycastHit(out var groundedRaycastHit);
        if (groundedRaycastHit.collider == null) return; 
        SetAnimatorMomentum();
        var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
        var flattenedNormal = new Vector3(groundedRaycastHit.normal.x, 0, groundedRaycastHit.normal.z);
        var forward = Quaternion.Euler(0f, 90f * rotationMod, 0f) * flattenedNormal;
        var lookRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);
        transform.position +=
            ComputeCollisionMove(-flattenedNormal * (Time.deltaTime * FlankWallVacuumStrength));
        
        HandleCollisionMove(0.15f);

    }
    
    private void SlideDownOnUpdate()
    {
        GetGroundedRaycastHit(out var groundedRaycastHit);
        var flattenedNormal = new Vector3(groundedRaycastHit.normal.x, 0, groundedRaycastHit.normal.z);
        
        var lookRotation = Quaternion.LookRotation(flattenedNormal, Vector3.up);
        Animator.SetLayerWeight(1, 0);
        if (groundedRaycastHit.collider == null) return; 
        SetAnimatorMomentum();
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);
        transform.position +=
            ComputeCollisionMove(-flattenedNormal * (Time.deltaTime * FlankWallVacuumStrength));
        
        HandleCollisionMove(0.15f);

    }

    private void SlideConfigure()
    {
        Machine.Configure(PlayerFsmState.Slide)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat,
                @params => !IsSlideTrigger(@params))
            .PermitIf(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.FallAfterSlide, _ => YVelocity < 0.5f, 5)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInCurrentState() > 0.25f)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .OnExitFrom(PlayerFsmTrigger.Jump, _ =>
            {
                // var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
                // var forward = Quaternion.Euler(0f, WallrunJumpAngle * rotationMod, 0f) * _currentFlankWallNormal;
                // transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            });
        // .OnExitFrom(GravityFsmTrigger.StartFrameAerial, _ => { _momentum = 7f; });


        Machine.Configure(PlayerFsmState.SlideLateral)
            .SubstateOf(PlayerFsmState.Slide)
            .OnEntry(@params =>
            {
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;

                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                IncrementCombo();

                if (@params is not RaycastHitParam raycastHitParam) return;
                var flattenedNormal = new Vector3(raycastHitParam.Hit.normal.x, 0, raycastHitParam.Hit.normal.z);
                var signedAngle = Vector3.SignedAngle(flattenedNormal, transform.forward, Vector3.up);
                bool flip = signedAngle > 0;
                _currentFlankType = flip ? FlankType.Right : FlankType.Left;
                Animator.SetFloat("Flip", flip ? 0f : 1f);
            });
        
        Machine.Configure(PlayerFsmState.SlideDown)
            .SubstateOf(PlayerFsmState.Slide)
            .OnEntry(@params =>
            {
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                
                // if (@params is not RaycastHitParam raycastHitParam) return;
                // var flattenedNormal = new Vector3(raycastHitParam.Hit.normal.x, 0, raycastHitParam.Hit.normal.z);
                // var signedAngle = Vector3.SignedAngle(flattenedNormal, transform.forward, Vector3.up);
                // bool flip = signedAngle > 0;
                // _currentFlankType = flip ? FlankType.Right : FlankType.Left;
                // Animator.SetFloat("Flip", flip ? 0f : 1f);
            });

        Machine.Configure(PlayerFsmState.SlideInteractable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.SlideLateral,
                @params => IsSlideTrigger(@params) &&
                           !(Machine.IsInState(PlayerFsmState.Jump) && TimeInCurrentState() < 0.25f) &&
                           !(Machine.IsInState(PlayerFsmState.Skip) && TimeInCurrentState() < 0.3f), 6);
    }
    
    private bool IsRaycastHitParamShallow(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        if (raycastHitParam.Hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return false;
        raycastHitParam.Hit.collider.Raycast(new Ray(raycastHitParam.Hit.point + Vector3.up, -Vector3.up), out var hit, 2f);
        var angle = Vector3.Angle(hit.normal, Vector3.up);
        Debug.DrawRay(raycastHitParam.Hit.point, hit.normal, Color.yellow, 1f);
        return angle < 40f;
    }
    
}
