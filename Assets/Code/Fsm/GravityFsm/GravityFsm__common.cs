using UnityEngine;

public abstract partial class GravityFsm
{
    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    private const float GroundedYPositionLerpStrength = 50f;
    private const float GroundedRaycastLength = 0.5f;
    private const float GroundedRaycastForwardOffset = 0.2f;
    private const float GroundedRaycastMaximumAngle = 70f;

    
    // Source of truth for whether the GravityFsm is on the ground. Checks down a certain distance with a slightly
    // forward origin to prevent corner clipping. To account for being potentially both above or below the ground
    // plane, the raycast origin is placed above the true position and then casted downwards with double length.
    // Ignores slopes with an angle greater than some threshhold.
    private bool GetGroundedRaycastHit(out RaycastHit hit)
    {
        var raycastLength = GroundedRaycastLength * GetRaycastTimeModifier();
        var forward = transform.forward * (GroundedRaycastForwardOffset * GetRaycastTimeModifier());
        if (Physics.Raycast(transform.position + Vector3.up * raycastLength + forward, -Vector3.up, out hit,
                raycastLength * 2f, ~0, QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            return slope < GroundedRaycastMaximumAngle;
        }

        return false;
    }

    // Tracks the height of the GravityFsm's current fall, for use in "hard landings".
    protected float CurrentFallDistance()
    {
        var diff = transform.position.y - LastUpwardsY;
        return diff;
    }
    
    // Keeps YVelocity clamped to maximum and updates LastUpwardsY if necessary
    private void UpdateYVelocityMetadata()
    {
        YVelocity = Mathf.Max(YVelocity, MinYVelocity);
        if (YVelocity > 0 || Machine.IsInState(GravityFsmState.Grounded)) LastUpwardsY = transform.position.y;
    }
}