using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{

    private void SwimOnUpdate()
    {
        if (_playerInput.actions["Sprint"].IsPressed()) isSprinting = true;
        
        HandleInputMomentumChange();
        HandleTurning(1f, false, 1f, false, isSprinting ? 0.5f : 1f);
        

        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
    }
    
    
    private void SwimSurfaceRiseOnUpdate()
    {
        if (WaterRaycast(out var hit))
        {
            var desiredYVelocity = Mathf.Lerp(0f, 25f, Mathf.InverseLerp(4f, -0.80f, hit.distance));
            YVelocity = Mathf.Lerp(YVelocity, desiredYVelocity,
                Time.deltaTime * Mathf.Lerp(1f, 20f, Mathf.InverseLerp(0, 0.45f, TimeInCurrentState())));
        }
        
        HandleCollisionMove(Mathf.Lerp(0.1925f, 0.0925f * 0.75f, Mathf.InverseLerp(0, 0.4f, TimeInCurrentState())));
    }
    
    private void SwimSurfaceOnUpdate()
    {
        if (WaterRaycast(out var hit))
        {
            transform.position += ComputeCollisionMove((hit.point + Vector3.up * -0.80f) - transform.position);
        }

        OnPlayerWakeGenerated?.Invoke(GetRipplePosition(), Mathf.Lerp(0.075f, 0.075f, ComputeMomentumWeight()), Mathf.Lerp(0.0011f, 0.0009f, ComputeMomentumWeight()));
        if (!_swimSurfaceRippleQueued) StartCoroutine(SwimSurfaceRippleCoroutine());
        HandleCollisionMove(0.0925f * 0.75f);
    }

    private IEnumerator SwimSurfaceRippleCoroutine()
    {
        _swimSurfaceRippleQueued = true;
        OnPlayerRippleGenerated?.Invoke(GetRipplePosition(), 0.5f, 0.001f);
        yield return new WaitForSeconds(Mathf.Lerp(SwimSurfaceRippleTimer * 7f, SwimSurfaceRippleTimer, ComputeMomentumWeight()));
        _swimSurfaceRippleQueued = false;
    }

    private Vector3 GetRipplePosition()
    {
        return transform.position + transform.forward * Mathf.Lerp(0f, 1f, ComputeMomentumWeight());
    }

    private void SwimConfigure()
    {
        Machine.Configure(PlayerFsmState.Swim)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(GravityFsmState.DontLoseYVelocity)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
            });
        
        Machine.Configure(PlayerFsmState.SwimSurfaceRise)
            .PermitIf(PlayerFsmTrigger.SwimTriggerRaycastHit, PlayerFsmState.SwimSurface, @params => IsSwimTriggerAtSurface(@params) && YVelocity < 2f)
            .SubstateOf(PlayerFsmState.Swim)
            .OnEntry(_ =>
            {
                if (WaterRaycast(out var hit))
                {
                    _splashParticles.transform.position = hit.point;
                    _splashParticles.Play();
                }
                OnPlayerRippleGenerated?.Invoke(transform.position, 1.0f, 0.005f);
                OnPlayerWakeGenerated?.Invoke(transform.position, 1.0f, 0.0025f);
            });
        
        Machine.Configure(PlayerFsmState.SwimSurface)
            .OnEntry(_ =>
            {
                YVelocity = 0;
            })
            .SubstateOf(PlayerFsmState.Swim);

        Machine.Configure(PlayerFsmState.DiveFall)
            .SubstateOf(PlayerFsmState.Fall);
    }


    private bool IsSwimTrigger(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        var distance = raycastHitParam.Hit.distance;
        return distance < 3f;
    }
    
    private bool IsSwimTriggerAtSurface(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        var distance = raycastHitParam.Hit.distance;
        return distance > 3f;
    }
    
}