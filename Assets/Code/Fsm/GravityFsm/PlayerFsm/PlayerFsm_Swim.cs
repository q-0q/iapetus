using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{
    private void PlungeOnUpdate()
    {
        
    }

    private void SwimOnUpdate()
    {
        if (_playerInput.actions["Sprint"].IsPressed()) isSprinting = true;
        
        HandleInputMomentumChange();
        HandleTurning(1f, false, 1f, false, isSprinting ? 0.5f : 1f);
        HandleCollisionMove(0.25f);

        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
    }
    
    
    private void SwimSurfaceRiseOnUpdate()
    {
        if (WaterRaycast(out var hit))
        {
            var desiredYVelocity = Mathf.Lerp(0f, 25f, Mathf.InverseLerp(4f, -2f, hit.distance));
            YVelocity = Mathf.Lerp(YVelocity, desiredYVelocity,
                Time.deltaTime * Mathf.Lerp(1f, 20f, Mathf.InverseLerp(0, 0.35f, TimeInCurrentState())));
        }
    }

    private void SwimConfigure()
    {
        Machine.Configure(PlayerFsmState.Swim)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .SubstateOf(GravityFsmState.DontLoseYVelocity);
        
        Machine.Configure(PlayerFsmState.SwimSurfaceRise)
            .PermitIf(PlayerFsmTrigger.SwimTriggerRaycastHit, PlayerFsmState.SwimSurface, @params => IsSwimTriggerAtSurface(@params) && YVelocity < 1f)
            .SubstateOf(PlayerFsmState.Swim);
        
        Machine.Configure(PlayerFsmState.SwimSurface)
            .OnEntry(_ =>
            {
                YVelocity = 0;
            })
            .SubstateOf(PlayerFsmState.Swim);
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