using UnityEngine;

public abstract partial class GravityFsm
{
    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    protected float GroundForwardSlope;
    private Vector3 _previousFailsafePosition;
    
    private const float GroundedYPositionLerpStrength = 50f;
    private const float GroundedRaycastLength = 0.75f;
    private const float GroundedRaycastForwardOffset = 0.05f;
    private const float GroundedRaycastMaximumAngle = 70f;
    
    protected Transform _parentTransform;
    protected Vector3 _previousParentTransformPosition;
    protected Quaternion _previousParentRotation;
    
    private const float CollisionMoveSphereCastRadius = 0.4f;
    private const float GroundCollisionMoveSphereCastHeight = 0.95f;
    private const float FallingCollisionMoveSphereCastHeight = -0.5f;
    private const float FallingCollisionMoveSphereCastHeightYVelocityThreshhold = -10f;
    private const float CollisionMoveSphereCastDistance = 0.45f;
    
    
    private const float FailsafeSphereRadius = 0.15f;
    private const float FailsafeSphereYOffset = 0.6f;
    private const float FailsafeSphereForwardOffset = 0.1f;

    
    
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
    
    protected Vector3 ComputeCollisionMove(Vector3 desiredMove)
    {
        var output = desiredMove;
        
        // Radius of your character (adjust as needed)
        var backwardsPadding = 0.45f;
        float radius = CollisionMoveSphereCastRadius;
        float castDistance = (CollisionMoveSphereCastDistance * GetRaycastTimeModifier()) - (radius * 0.45f) + backwardsPadding;

        Vector3 position = transform.position + Vector3.up * (YVelocity > FallingCollisionMoveSphereCastHeightYVelocityThreshhold
                               ? GroundCollisionMoveSphereCastHeight
                               : FallingCollisionMoveSphereCastHeight);
        Vector3 direction = output.normalized;

        // SphereCast to account for player volume
        if (Physics.SphereCast(position - transform.forward * backwardsPadding, radius, direction, out RaycastHit hit, castDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            
            // First collision: slide along the surface
            Vector3 firstNormal = hit.normal;
            output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(firstNormal, Vector3.up));


            // Cast again in the new direction to handle corner (second surface)
            if (Physics.SphereCast(position - firstNormal * backwardsPadding, radius, output.normalized, out RaycastHit secondHit, output.magnitude + backwardsPadding))
            {
                Vector3 secondNormal = secondHit.normal;

                // Slide again
                output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(secondNormal, Vector3.up));
                
                if (output.magnitude < 0.01f)
                {
                    output = Vector3.zero;
                }
            }
        }
        
        return output;
    }
    
    private void HandleFailsafe()
    {
        if (Machine.IsInState(GravityFsmState.IgnoreFailsafe)) return;
        
        if (Physics.CheckSphere(transform.position + 
                                (transform.up * FailsafeSphereYOffset) +
                                (transform.forward * FailsafeSphereForwardOffset), 
                FailsafeSphereRadius,
                GetEnvironmentalLayermask(), 
                QueryTriggerInteraction.Ignore))
        {
            transform.position = _previousFailsafePosition;
        }
        else
        {
            _previousFailsafePosition = transform.position;
        }
    }
    

}