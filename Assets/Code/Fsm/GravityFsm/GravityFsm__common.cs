using UnityEngine;

public abstract partial class GravityFsm
{
    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    protected float GroundForwardSlope;
    private Transform _parentTransform;
    private Vector3 _previousParentTransformPosition;
    private Quaternion _previousParentRotation;
    
    private const float GroundedYPositionLerpStrength = 50f;
    private const float GroundedRaycastLength = 0.75f;
    private const float GroundedRaycastForwardOffset = 0.05f;
    private const float GroundedRaycastMaximumAngle = 70f;
    
    private bool GetGroundedRaycastHit(out RaycastHit hit)
    {
        var raycastLength = GroundedRaycastLength * GetRaycastTimeModifier();
        var forward = transform.forward * (GroundedRaycastForwardOffset * GetRaycastTimeModifier());
        if (Physics.SphereCast(transform.position + Vector3.up * raycastLength + forward, 0.35f, -Vector3.up, out hit,
                raycastLength * 2f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            return slope < GroundedRaycastMaximumAngle;
        }

        return false;
    }
    
    protected float CurrentFallDistance()
    {
        var diff = transform.position.y - LastUpwardsY;
        return diff;
    }
    
    private void UpdateYVelocityMetadata()
    {
        YVelocity = Mathf.Max(YVelocity, MinYVelocity);
        if (YVelocity > 0 || Machine.IsInState(GravityFsmState.Grounded)) LastUpwardsY = transform.position.y;
    }
}