using System;
using Code.Misc;
using Code.TriggerParams;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using Wasp;

public partial class PlayerFsm
{
    public static event Action<bool> OnPlayerEnteredSlideLateral;
    
    private void SlideLateralOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        SetAnimatorMomentum();
        var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
        var flattenedNormal = new Vector3(_currentSlideNormal.x, 0, _currentSlideNormal.z);
        var forward = Quaternion.Euler(0f, 90f * rotationMod, 0f) * flattenedNormal;
        var lookRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);
        
        HandleCollisionMove(0.15f);

    }
    
    private void FallAfterSlideLateralOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        SetAnimatorMomentum();
        var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
        var flattenedNormal = new Vector3(_currentSlideNormal.x, 0, _currentSlideNormal.z);
        var forward = Quaternion.Euler(0f, 90f * rotationMod, 0f) * flattenedNormal;
        var lookRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);
        
        HandleCollisionMove(0.15f);
        
        transform.position +=
            ComputeCollisionMove(flattenedNormal.normalized * (Time.deltaTime * FlankWallVacuumStrength));

    }
    
    private void SlideDownOnUpdate()
    {

        
        var flattenedNormal = new Vector3(_currentSlideNormal.x, 0, _currentSlideNormal.z).normalized;
        
        var lookRotation = Quaternion.LookRotation(flattenedNormal, Vector3.up);
        Animator.SetLayerWeight(1, 0);
 
        SetAnimatorMomentum();
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);


        
        HandleCollisionMove(0.15f);

    }

    private void SlideOnUpdate()
    {
        GetGroundedRaycastHit(out var groundedRaycastHit);
        if (groundedRaycastHit.collider != null)
        {
            if (IsSlideTriggerCore(groundedRaycastHit)) _currentSlideNormal = groundedRaycastHit.normal;
        } 
        
        var flattenedNormal = new Vector3(_currentSlideNormal.x, 0, _currentSlideNormal.z).normalized;

        var vacuumRaycastOrigin = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(vacuumRaycastOrigin, -flattenedNormal, out RaycastHit vacuumHit, 3f, GetEnvironmentalLayermask()))
        {
            var distance = Vector3.Distance(transform.position, vacuumHit.point);
            var distanceMod = Mathf.Lerp(0.1f, 1f, Mathf.InverseLerp(2.5f, 3.25f, distance));
            transform.position +=
                ComputeCollisionMove(-flattenedNormal.normalized * (Time.deltaTime * FlankWallVacuumStrength * distanceMod));
        }
    }

    private void SlideConfigure()
    {
        Machine.Configure(PlayerFsmState.Slide)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat,
                @params => !IsSlideTrigger(@params) && TimeInCurrentState() >= 0.25f)
            .PermitIf(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.FallAfterSlide, _ => YVelocity < 0.5f, 5)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat,
                _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInCurrentState() > 0.4f)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && TimeInCurrentState() > 0.25f)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                @params => (!IsSlideTrigger(@params) && CurrentFallDistance() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum), 4)
            .OnEntry(_ =>
            {
                
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(slideFmodInstance, gameObject);
                slideFmodInstance.start();
            })
            .OnEntry(@params =>
            {
                if (@params is not RaycastHitParam raycastHitParam) return;
                _currentSlideNormal = raycastHitParam.Hit.normal;
            })
            .OnExitFrom(PlayerFsmTrigger.Jump, _ =>
            {
                var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
                var flattenedNormal = new Vector3(_currentSlideNormal.x, 0, _currentSlideNormal.z);
                var forward = Quaternion.Euler(0f, WallrunJumpAngle * rotationMod, 0f) * flattenedNormal;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            })
            .OnExit(_ =>
            {
                slideFmodInstance.stop(STOP_MODE.ALLOWFADEOUT);
            });
        // .OnExitFrom(GravityFsmTrigger.StartFrameAerial, _ => { _momentum = 7f; });


        Machine.Configure(PlayerFsmState.SlideLateral)
            .SubstateOf(PlayerFsmState.Slide)
            .PermitIf(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.FallAfterSlideLateral, _ => YVelocity < 0.5f, 6)
            .OnEntry(@params =>
            {
                YVelocity += 10f;
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;

                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                // IncrementCombo();

                if (@params is not RaycastHitParam raycastHitParam) return;
                var flattenedNormal = new Vector3(raycastHitParam.Hit.normal.x, 0, raycastHitParam.Hit.normal.z);
                var signedAngle = Vector3.SignedAngle(flattenedNormal, transform.forward, Vector3.up);
                bool flip = signedAngle > 0;
                OnPlayerEnteredSlideLateral?.Invoke(flip);
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
                _momentum = 3f;
                _currentComboLength = 0;
                OnPlayerComboReset?.Invoke();
            });

        Machine.Configure(PlayerFsmState.SlideInteractable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.SlideLateral,
                @params =>
                {
                    return IsSlideTrigger(@params) &&
                        !(Machine.IsInState(PlayerFsmState.Jump) && TimeInCurrentState() < 0.25f) &&
                        !(Machine.IsInState(PlayerFsmState.Skip) && TimeInCurrentState() < 0.3f);
                }, 6)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.SlideDown,
                @params =>
                {
                    if (!IsSlideTrigger(@params)) return false;
                    if (@params is not RaycastHitParam raycastHitParam) return false;

                    var angle = Vector3.Angle(transform.forward,
                        new Vector3(raycastHitParam.Hit.normal.x, 0, raycastHitParam.Hit.normal.z));

                    if (_momentum < 6f) return true;
                    if (angle < 30f) return true;
                    if (angle > 150f) return true;
                    return false;
                }, 7);
        
        
        Machine.Configure(PlayerFsmState.FallAfterSlide)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall);
        
        Machine.Configure(PlayerFsmState.FallAfterSlideLateral)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall);
    }
    
    
    private bool IsSlideTrigger(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        return IsSlideTriggerCore(raycastHitParam.Hit);
    }
    
    private bool IsSlideTriggerCore(RaycastHit hit)
    {
        var playerSlideIndicator = hit.transform.GetComponent<PlayerSlideIndicator>();
        if (playerSlideIndicator == null) return false;

        var ray = hit;
            
        if (Physics.Raycast(hit.point + Vector3.up, Vector3.down, out RaycastHit recast, 2f,
                GetEnvironmentalLayermask()))
        {
            ray = recast;
        }

        var angle = Vector3.Angle(ray.normal, ray.transform.up);
        
        if (Machine.IsInState(PlayerFsmState.Slide))
        {
            return angle < 100f;
        }
        else
        {
            return angle < 10f;
        }
    }
    
}
