using UnityEngine;

public abstract partial class GravityFsm
{
    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    
    private bool GetGroundedRaycastHit(out RaycastHit hit)
    {
        var raycastLength = 0.5f * GetRaycastTimeModifier();
        var forward = transform.forward * (0.2f * GetRaycastTimeModifier());
        Debug.DrawLine(transform.position + Vector3.up * raycastLength + forward,
            transform.position + Vector3.up * raycastLength - Vector3.up * (raycastLength * 1.3f) + forward, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up * raycastLength + forward, -Vector3.up, out hit,
                raycastLength * 2f, ~0, QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            return slope < 70f;
        }

        return false;
    }

    protected float AirYDiff()
    {
        var diff = transform.position.y - LastUpwardsY;
        return diff;
    }
}