using UnityEngine;

public abstract partial class GravityFsm
{
    // Soft-snap y position onto the grounded raycast point via Lerp
    private void GroundedOnUpdate()
    {
        YVelocity = 0;
        if (GetGroundedRaycastHit(out var hit))
        {
            var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * GroundedYPositionLerpStrength);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        UpdateYVelocityMetadata();
    }

    private void GroundedConfigure()
    {
        Machine.Configure(GravityFsmState.Grounded);
    }
}