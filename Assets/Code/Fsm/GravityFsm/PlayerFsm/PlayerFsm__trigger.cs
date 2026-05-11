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
            if (!CutsceneManager.Singleton.IsCutsceneJumpDisabled()) Machine.Fire(PlayerFsmTrigger.Jump);
        }
        
        if (_inputBuffer.IsBuffered("Dash"))
        {
            Machine.Fire(PlayerFsmTrigger.Dash);
        }
        
        
        if (_inputBuffer.IsBuffered("Attack"))
        {
            Machine.Fire(PlayerFsmTrigger.Attack);
        }
        
        if (_inputBuffer.IsBuffered("Inventory"))
        {
            Machine.Fire(PlayerFsmTrigger.Inventory);
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


        var walkToPositionTargetDistance = Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(walkToPositionTarget.x, walkToPositionTarget.z));
        if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTarget);
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTargetRanged);
        } else if (walkToPositionTargetDistance < ArriveAtWalkPositionTargetRangedDistance)
        {
            Machine.Fire(PlayerFsmTrigger.ArriveAtWalkToPositionTargetRanged);
        } 

        FireFaceTriggers();
        var flank = FireFlankTriggers();

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
            
            if (YVelocity < 0 && Physics.Raycast(transform.position, Vector3.down, 2f * GetRaycastTimeModifier(), GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)) continue;
            var param = new PitonParam() { Piton = neighbor.transform.parent};
            Machine.Fire(PlayerFsmTrigger.EnterPitonTrigger, param);
        }

        FireSwimTrigger();

        if (YVelocity < 15f && _momentum > 12f && !flank)
        {
            if (Physics.Raycast(transform.position, Vector3.down + transform.forward, out var hit, 55f, ~LayerMask.GetMask("PlayerClothCollider", "PlayerCloth", "Player", "FoliageSystems")))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water")) Machine.Fire(PlayerFsmTrigger.IsAboveWater);
            }
        }
        
        
        foreach (var neighbor in Physics.OverlapSphere(transform.position, 7f, LayerMask.GetMask("RopeSwing"), QueryTriggerInteraction.Collide))
        {
            if (YVelocity < 0 && Physics.Raycast(transform.position, Vector3.down, 2f * GetRaycastTimeModifier(), GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)) continue;
            var ropeSwing = neighbor.transform.parent.parent.GetComponent<RopeSwing>();
            var pos = ropeSwing.GetWorldspaceAttachPoint(transform.position);
            var relativePos = transform.InverseTransformPoint(pos);

            var maxZ = Mathf.Lerp(2f, 6f, Mathf.InverseLerp(0.3f, 0.7f, ComputeMomentumWeight()));
            if (relativePos.z < -1f) continue;
            if (relativePos.z > maxZ) continue;
            if (Mathf.Abs(relativePos.x) > 3f) continue;
            if (Mathf.Abs(relativePos.y) > 3f) continue;
            
            var param = new RopeSwingHitParam() { RopeSwing = ropeSwing};
            Machine.Fire(PlayerFsmTrigger.EnterRopeSwingTrigger, param);
        }
        
        if (PressRaycast(out _)) Machine.Fire(PlayerFsmTrigger.Press);
        
        if (!Physics.Raycast(transform.position + transform.forward * (GetRaycastTimeModifier() * Mathf.Lerp(0, 6f, ComputeMomentumWeight())), Vector3.down, out var hit2, 25f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.IsAboveLongFall);
        }
        
        foreach (var neighbor in Physics.OverlapSphere(transform.position, 2.75f, LayerMask.GetMask("MinorLeylineTrigger"), QueryTriggerInteraction.Collide))
        {
            _currentMinorLeyline = neighbor.transform.parent.parent.GetComponent<MinorLeyline>();
            _currentMinorLeylineTrigger = neighbor.transform;
            Machine.Fire(PlayerFsmTrigger.MinorLeylineTrigger);
        }
    }

    private bool PressRaycast(out RaycastHit hit)
    {
        var b = (Physics.Raycast(transform.position + (Vector3.up * FaceHighLedgeHeight) - (transform.forward),
            transform.forward, out hit,
            8f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore));

        if (!b) return false;
        var slope = Vector3.Angle(hit.normal, Vector3.up);
        if (slope < 70f) return false;
        var angle = Vector3.SignedAngle(-hit.normal, transform.forward, Vector3.up);
        var angleWeight = Mathf.InverseLerp(0, 40f, Mathf.Abs(angle));
        var distanceThreshhold = Mathf.Lerp(1.75f, 2.15f + (Machine.IsInState(PlayerFsmState.Press) ? 4f : 0f), angleWeight);
        return hit.distance < distanceThreshhold;
    }

    private bool ProbeHighLedge()
    {
        var origin = transform.position + Vector3.up * (FaceWallHeight + GetCurrentDashRaycastHeightOffset());
        if (Physics.Raycast(origin,
                transform.forward,
                out var hit, 2f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, transform.forward, Color.magenta, 1f);
            return true;
        }

        return false;
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
            if (slope > minSlope - 30f) Machine.Fire(Vector3.Angle(-hit.normal, transform.forward) < FaceWallStrictMaximumAngle
                ? PlayerFsmTrigger.FaceWallStrict
                : PlayerFsmTrigger.FaceWall, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up *
                       (FaceHighLedgeHeight + GetCurrentDashRaycastHeightOffset()), transform.forward, 
                       out hit, forwardRaycastDistance + skew, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            if (ProbeHighLedge()) return;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return;
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > minSlope) Machine.Fire(PlayerFsmTrigger.FaceHighLedge, new RaycastHitParam() { Hit = hit});
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceLedgeHeight, transform.forward,
                       out hit, forwardRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) && Vector3.Angle(-hit.normal, transform.forward) < FaceWallMaximumAngle)
        {
            if (ProbeHighLedge()) return;
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

    private bool FireFlankTriggers()
    {
        RaycastHit hit;
        var maximumFlankRaycastDistance = MaximumFlankWallDistance;
        var flankRaycastOrigin = transform.position + Vector3.up * FlankWallHeight;
        if 
            (Physics.Raycast(flankRaycastOrigin, transform.right,
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore) 
             && IsHitValidFlank(hit, true) && _previousWallrunSide != FlankType.Right)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return false;
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Right;
            _currentWallrunTransform = hit.transform;
            Animator.SetFloat("Flip", 0);
            return true;

        } else if 
            (Physics.Raycast(flankRaycastOrigin, -transform.right, 
                 out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)
             && IsHitValidFlank(hit, false) && _previousWallrunSide != FlankType.Left)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide")) return false;
            Machine.Fire(PlayerFsmTrigger.FlankWall, new RaycastHitParam() { Hit = hit});
            _currentFlankWallNormal = hit.normal;
            _currentFlankType = FlankType.Left;
            _currentWallrunTransform = hit.transform;
            Animator.SetFloat("Flip", 1);
            return true;
        }
        else if (!Physics.Raycast(flankRaycastOrigin + (Vector3.up * FlankWallOpenYOffset), transform.right,
                     out hit, maximumFlankRaycastDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FlankOpen);
            return false;
        }
        
        
        Debug.DrawRay(flankRaycastOrigin, transform.right * maximumFlankRaycastDistance, Color.blue);
        Debug.DrawRay(flankRaycastOrigin, -transform.right * maximumFlankRaycastDistance, Color.blue);
        
        return false;
    }
}