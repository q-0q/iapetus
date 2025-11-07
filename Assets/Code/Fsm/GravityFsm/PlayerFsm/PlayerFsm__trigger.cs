using System;
using Code.TriggerParams;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerFsm
{
   
    public override void OnFireTriggers()
    {
        
        base.OnFireTriggers();
        
        if (_inputBuffer.IsBuffered("Jump"))
        {
            Machine.Fire(PlayerFsmTrigger.Jump);
        }
        
        if (_inputBuffer.IsBuffered("Attack"))
        {
            Machine.Fire(PlayerFsmTrigger.Attack);
        }
        
        // if (_inputBuffer.IsBuffered("Interact"))
        // {
        //     var neighbors = Physics.OverlapSphere(transform.position, InteractionDistance,
        //         LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Collide);
        //     foreach (var neighbor in neighbors)
        //     {
        //         var deltaY = neighbor.transform.position.y - transform.position.y;
        //         neighbor.TryGetComponent(out Interactable interactionCollider);
        //         var param = new InteractableParam() { Interactable = interactionCollider };
        //         Machine.Fire(PlayerFsmTrigger.InteractWithSwitch, param);
        //     }
        // }
        
        if (_inputBuffer.IsBuffered("Dash"))
        {
            Machine.Fire(PlayerFsmTrigger.Dash);
        }
        
        var v3 = GetInputMovementVector3();
        var angle = Vector3.Angle(v3.normalized, transform.forward.normalized);
        if (angle > HardTurnMinimumAngle && _momentum > HardTurnMinimumMomentum)
        {
            Machine.Fire(PlayerFsmTrigger.HardTurn);
        }

        if (_momentum < NoMomentumThreshold)
        {
            Machine.Fire(PlayerFsmTrigger.NoMomentum);
        }


        var walkToPositionTargetDistance = Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_walkToPositionTarget.x, _walkToPositionTarget.z));
        if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTarget);
        } else if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetRangedDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTargetRanged);
        } 

        FireFaceTriggers();
        FireFlankTriggers();
    }

    private void FireFaceTriggers()
    {
        var forwardRaycastDistance = ComputeDynamicForwardRaycastDistance();
        var skew = FaceRaycastSkew * GetRaycastTimeModifier();
        if (Physics.Raycast(transform.position + Vector3.up * (FaceWallHeight + GetCurrentDashRaycastHeightOffset()), transform.forward, 
                out var hit, forwardRaycastDistance + skew * 2f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > 70f) Machine.Fire(Vector3.Angle(-hit.normal, transform.forward) < FaceWallStrictMaximumAngle
                ? PlayerFsmTrigger.FaceWallStrict
                : PlayerFsmTrigger.FaceWall, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up *
                       (FaceHighLedgeHeight + GetCurrentDashRaycastHeightOffset()), transform.forward, 
                       out hit, forwardRaycastDistance + skew, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > 70f) Machine.Fire(PlayerFsmTrigger.FaceHighLedge, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceLedgeHeight, transform.forward,
                       out hit, forwardRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > 70f) Machine.Fire(PlayerFsmTrigger.FaceLedge, new RaycastHitParam() { Hit = hit});
        }
        else
        {
            Machine.Fire(PlayerFsmTrigger.FaceOpen);
        }

        if (!Physics.Raycast(transform.position + Vector3.up * FaceWallHeight, transform.forward,
                out hit, (forwardRaycastDistance + skew * 2f) + 0.25f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FaceOpenLenient);
        }
        
        Debug.DrawRay(transform.position + Vector3.up * (FaceWallHeight  + GetCurrentDashRaycastHeightOffset()), transform.forward *
            (forwardRaycastDistance + skew * 2f), Color.red);
        Debug.DrawRay(transform.position + Vector3.up * (FaceHighLedgeHeight + GetCurrentDashRaycastHeightOffset()), transform.forward *
            (forwardRaycastDistance + skew), Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up * FaceLedgeHeight, transform.forward, Color.cyan);
    }

    private void FireFlankTriggers()
    {
        RaycastHit hit;
        var maximumFlankRaycastDistance = MaximumFlankWallDistance;
        var flankRaycastOrigin = transform.position + Vector3.up * FlankWallHeight;
        if 
            (Physics.Raycast(flankRaycastOrigin, transform.right,
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) 
             && IsHitValidFlank(hit, true) && _previousWallrunSide != FlankType.Right)
        {
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Right;
            Animator.SetFloat("Flip", 0);

        } else if 
            (Physics.Raycast(flankRaycastOrigin, -transform.right, 
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)
             && IsHitValidFlank(hit, false) && _previousWallrunSide != FlankType.Left)
        {
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Left;
            Animator.SetFloat("Flip", 1);
        }
        else if (!Physics.Raycast(flankRaycastOrigin + (Vector3.up * FlankWallOpenYOffset), transform.right,
                     out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FlankOpen);
        }
        
        Debug.DrawRay(flankRaycastOrigin, transform.right * maximumFlankRaycastDistance, Color.blue);
        Debug.DrawRay(flankRaycastOrigin, -transform.right * maximumFlankRaycastDistance, Color.blue);
    }
}