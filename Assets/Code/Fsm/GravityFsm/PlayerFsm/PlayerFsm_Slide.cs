using Code.Misc;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using Wasp;

public partial class PlayerFsm
{
    private void SlideOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        GetGroundedRaycastHit(out var groundedRaycastHit);
        if (groundedRaycastHit.collider == null) return; 
        // groundedRaycastHit.collider.Raycast(new Ray(groundedRaycastHit.point + Vector3.up, -Vector3.up), out var hit, 2f);
        var forward = new Vector3(groundedRaycastHit.normal.x, 0, groundedRaycastHit.normal.z);
        var destinationRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, destinationRotation, Time.deltaTime * 10f);
        var speed = Mathf.Lerp(5f, 20f, Mathf.InverseLerp(0, 1f, TimeInCurrentState()));
        transform.position += ComputeCollisionMove(forward * (speed * Time.deltaTime));

    }

    private void SlideConfigure()
    {
        Machine.Configure(PlayerFsmState.Slide)
            .SubstateOf(GravityFsmState.Grounded)
            // .SubstateOf(PlayerFsmState.Interactable)
            .PermitIf(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.FallAfterSlide, _=> true, 5)
            // .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat, IsRaycastHitParamShallow, 2);
    }

    private bool IsRaycastHitParamSteep(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        return raycastHitParam.Hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide");
        if (!raycastHitParam.Hit.collider.Raycast(new Ray(raycastHitParam.Hit.point + Vector3.up, -Vector3.up),
            out var hit, 2f)) return false;
        return false;
        var angle = Vector3.Angle(hit.normal, Vector3.up);
        Debug.DrawRay(raycastHitParam.Hit.point, hit.normal, Color.yellow, 1f);
        return angle > 50f;
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
