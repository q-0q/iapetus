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
        GetGroundedRaycastHit(out var groundedRaycastHit, out _);
        if (groundedRaycastHit.collider == null) return; 
        groundedRaycastHit.collider.Raycast(new Ray(groundedRaycastHit.point + Vector3.up, -Vector3.up), out var hit, 2f);
        var forward = new Vector3(hit.normal.x, 0, hit.normal.z);
        var destinationRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, destinationRotation, Time.deltaTime * 10f);
        transform.position += ComputeCollisionMove(forward * (5f * Time.deltaTime));

    }

    private void SlideConfigure()
    {
        Machine.Configure(PlayerFsmState.Slide)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.Interactable)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat, IsRaycastHitParamShallow, 2);
    }

    private bool IsRaycastHitParamSteep(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        if (!raycastHitParam.Hit.collider.Raycast(new Ray(raycastHitParam.Hit.point + Vector3.up, -Vector3.up),
            out var hit, 2f)) return false;
        var angle = Vector3.Angle(hit.normal, Vector3.up);
        Debug.DrawRay(raycastHitParam.Hit.point, hit.normal, Color.yellow, 1f);
        return angle > 50f;
    }
    
    private bool IsRaycastHitParamShallow(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        raycastHitParam.Hit.collider.Raycast(new Ray(raycastHitParam.Hit.point + Vector3.up, -Vector3.up), out var hit, 2f);
        var angle = Vector3.Angle(hit.normal, Vector3.up);
        Debug.DrawRay(raycastHitParam.Hit.point, hit.normal, Color.yellow, 1f);
        return angle < 40f;
    }
    
}
