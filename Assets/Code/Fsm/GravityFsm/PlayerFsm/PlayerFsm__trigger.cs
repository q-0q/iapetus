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
        
        if (_inputBuffer.IsBuffered("Dash"))
        {
            Machine.Fire(PlayerFsmTrigger.Dash);
        }
        
        
        if (_inputBuffer.IsBuffered("Attack"))
        {
            Machine.Fire(PlayerFsmTrigger.Attack);
        }
        
        var v3 = GetInputMovementVector3();
        var angle = Vector3.Angle(v3.normalized, transform.forward.normalized);
        if (angle > HardTurnMinimumAngle)
        {
            Machine.Fire(PlayerFsmTrigger.HardTurn);
        }

        var signedAngle = Vector3.SignedAngle(v3.normalized, transform.forward.normalized, Vector3.up);
        if (signedAngle > 70f)
        {
            Machine.Fire(PlayerFsmTrigger.SoftTurnLeft);
        } else if (signedAngle < -70f)
        {
            Machine.Fire(PlayerFsmTrigger.SoftTurnRight);
        }

        if (_momentum < NoMomentumThreshold)
        {
            Machine.Fire(PlayerFsmTrigger.NoMomentum);
        }


        var walkToPositionTargetDistance = Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_walkToPositionTarget.x, _walkToPositionTarget.z));
        if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTarget);
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTargetRanged);
        } else if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetRangedDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTargetRanged);
        } 

        FireFaceTriggers();
        FireFlankTriggers();

        if (IsInGust)
        {
            Machine.Fire(PlayerFsmTrigger.StartUpdraft);
        }
        else if (!IsInGust)
        {
            Machine.Fire(PlayerFsmTrigger.EndUpdraft);
        }

        foreach (var neighbor in Physics.OverlapSphere(transform.position, 2.75f, LayerMask.GetMask("Piton"), QueryTriggerInteraction.Collide))
        {
            var yDelta = neighbor.transform.position.y - transform.position.y;
            if (yDelta > 4f) continue;
            if (yDelta < -2f) continue;
            
            if (YVelocity < 0 && Physics.Raycast(transform.position, Vector3.down, 2f * GetRaycastTimeModifier(), GetEnvironmentalLayermask())) continue;
            var param = new PitonParam() { Piton = neighbor.transform.parent};
            Machine.Fire(PlayerFsmTrigger.EnterPitonTrigger, param);
        }

        FireSwimTrigger();

        if (YVelocity < 15f && _momentum > 12f)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 35f, ~LayerMask.GetMask("PlayerClothCollider", "PlayerCloth", "Player")))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water")) Machine.Fire(PlayerFsmTrigger.IsAboveWater);
            }
        }

    }

    private void FireFaceTriggers()
    {
        var forwardRaycastDistance = ComputeDynamicForwardRaycastDistance();
        var skew = FaceRaycastSkew * GetRaycastTimeModifier();
        var minSlope = 80f;
        if (Physics.Raycast(transform.position + Vector3.up * (FaceWallHeight + GetCurrentDashRaycastHeightOffset()), transform.forward, 
                out var hit, forwardRaycastDistance + skew * 2f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > minSlope) Machine.Fire(Vector3.Angle(-hit.normal, transform.forward) < FaceWallStrictMaximumAngle
                ? PlayerFsmTrigger.FaceWallStrict
                : PlayerFsmTrigger.FaceWall, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up *
                       (FaceHighLedgeHeight + GetCurrentDashRaycastHeightOffset()), transform.forward, 
                       out hit, forwardRaycastDistance + skew, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > minSlope) Machine.Fire(PlayerFsmTrigger.FaceHighLedge, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceLedgeHeight, transform.forward,
                       out hit, forwardRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > minSlope) Machine.Fire(PlayerFsmTrigger.FaceLedge, new RaycastHitParam() { Hit = hit});
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
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Right;
            _currentWallrunTransform = hit.transform;
            Animator.SetFloat("Flip", 0);

        } else if 
            (Physics.Raycast(flankRaycastOrigin, -transform.right, 
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)
             && IsHitValidFlank(hit, false) && _previousWallrunSide != FlankType.Left)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Left;
            _currentWallrunTransform = hit.transform;
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