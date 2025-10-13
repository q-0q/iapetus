using System;
using Code.TriggerParams;
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
        
        if (_inputBuffer.IsBuffered("Interact"))
        {
            var neighbors = Physics.OverlapSphere(transform.position, InteractionDistance,
                LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Collide);
            foreach (var neighbor in neighbors)
            {
                neighbor.TryGetComponent(out InteractionCollider interactionCollider);
                var param = new InteractionParam() { InteractionCollider = interactionCollider };
                Machine.Fire(PlayerFsmTrigger.InteractWithSwitch, param);
            }
        }
        
        // if (_inputBuffer.IsBuffered("Dash"))
        // {
        //     Machine.Fire(PlayerFsmTrigger.Dash);
        // }
        
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
        
        if (Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_walkToPositionTarget.x, _walkToPositionTarget.z)) < 1.5f)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTarget);
        }

        FireFaceTriggers();
        // FireFlankTriggers();
    }

    private void FireFaceTriggers()
    {
        var forwardRaycastDistance = ComputeDynamicForwardRaycastDistance();
        if (Physics.Raycast(transform.position + Vector3.up * FaceWallHeight, transform.forward, 
                out var hit, forwardRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            Machine.Fire(Vector3.Angle(-hit.normal, transform.forward) < FaceWallStrictMaximumAngle
                ? PlayerFsmTrigger.FaceWallStrict
                : PlayerFsmTrigger.FaceWall, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceHighLedgeHeight, transform.forward, 
                       out hit, forwardRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            Machine.Fire(PlayerFsmTrigger.FaceHighLedge, new RaycastHitParam() { Hit = hit});
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
        
        // Debug.DrawRay(transform.position + Vector3.up * FaceWallHeight, transform.forward * forwardRaycastDistance, Color.red);
        // Debug.DrawRay(transform.position + Vector3.up * FaceHighLedgeHeight, transform.forward * forwardRaycastDistance, Color.yellow);
        // Debug.DrawRay(transform.position + Vector3.up * FaceLedgeHeight, transform.forward * forwardRaycastDistance, Color.cyan);
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
            Machine.Fire(PlayerFsmTrigger.FlankWall);
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Right;
            Animator.SetFloat("Flip", 0);

        } else if 
            (Physics.Raycast(flankRaycastOrigin, -transform.right, 
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)
             && IsHitValidFlank(hit, false) && _previousWallrunSide != FlankType.Left)
        {
            Machine.Fire(PlayerFsmTrigger.FlankWall);
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