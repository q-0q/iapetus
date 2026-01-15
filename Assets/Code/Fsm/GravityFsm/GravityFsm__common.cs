
using UnityEngine;


public abstract partial class GravityFsm
{
    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    protected float GroundForwardSlope;
    private Collider _depenetrationCollider;
    protected bool IsInGust = false;
    private float _timeOfLastGustSinOffset;
    private float _timeAtTopOfGust;

    public Transform parentTransform;
    protected Vector3 _previousParentTransformPosition;
    protected Quaternion _previousParentRotation;
    public GravityFsmSpringCollider springCollider;
    

    private const float GroundedYPositionLerpStrength = 50f;
    protected const float GroundedRaycastLength = 0.5f;
    protected const float GroundedRaycastForwardOffset = 0.05f;

    private const float GroundedRaycastMaximumAngle = 65f;

    private const float CollisionMoveSphereCastRadius = 0.4f;
    private const float GroundCollisionMoveSphereCastHeight = 1.15f;
    private const float FallingCollisionMoveSphereCastHeight = 1.15f;
    private const float FallingCollisionMoveSphereCastHeightYVelocityThreshhold = -10f;
    private const float CollisionMoveSphereCastDistance = 0.45f;

    public enum GroundKind
    {
        Standard,
        Tightrope
    }


    protected bool GetGroundedRaycastHit(out RaycastHit hit, bool debug = false)
    {
        var raycastLength = GroundedRaycastLength * GetRaycastTimeModifier();
        var forward = transform.forward * (GroundedRaycastForwardOffset * GetRaycastTimeModifier());
        var f = 1f;
        var minDistance = 0.1f;
        if (Physics.SphereCast(transform.position + Vector3.up * (f * raycastLength) + forward, 0.35f, -Vector3.up,
                out hit,
                raycastLength * f * 2f, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            var slopeMinDistanceOffset =
                Mathf.Lerp(0, 5f, Mathf.InverseLerp(0f, 90f, Vector3.Angle(hit.normal, Vector3.up)));
            return Mathf.Abs(transform.position.y - hit.point.y) < minDistance;
        }

        // if (debug) Debug.Log("GetGrounded Spherecast failed");
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
        if (YVelocity > 0 || Machine.IsInState(GravityFsmState.Grounded))
            LastUpwardsY = transform.position.y;
    }

    protected Vector3 ComputeCollisionMove(Vector3 desiredMove)
    {
        var output = desiredMove;

        // Radius of your character (adjust as needed)
        var backwardsPadding = 0.45f;
        float radius = CollisionMoveSphereCastRadius;
        float castDistance = (CollisionMoveSphereCastDistance * GetRaycastTimeModifier()) - (radius * 0.45f) +
                             backwardsPadding;

        Vector3 position = transform.position + Vector3.up *
            (YVelocity > FallingCollisionMoveSphereCastHeightYVelocityThreshhold
                ? GroundCollisionMoveSphereCastHeight
                : FallingCollisionMoveSphereCastHeight);
        Vector3 direction = output.normalized;

        // SphereCast to account for player volume
        if (Physics.SphereCast(position - transform.forward * backwardsPadding, radius, direction, out RaycastHit hit,
                castDistance, GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            // First collision: slide along the surface
            Vector3 firstNormal = hit.normal;
            output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(firstNormal, Vector3.up).normalized);


            // Cast again in the new direction to handle corner (second surface)
            if (Physics.SphereCast(position - firstNormal * backwardsPadding, radius, output.normalized,
                    out RaycastHit secondHit, output.magnitude + backwardsPadding))
            {
                Vector3 secondNormal = secondHit.normal;

                // Slide again
                output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(secondNormal, Vector3.up).normalized);

                if (output.magnitude < 0.01f)
                {
                    output = Vector3.zero;
                }
            }
        }

        return output;
    }


    private void HandleDepenetration()
    {
        var neighbors = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 3f, 0.5f,
            GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject.layer == LayerMask.NameToLayer("Tightrope")) continue;
            if (Physics.ComputePenetration(_depenetrationCollider, _depenetrationCollider.transform.position,
                    _depenetrationCollider.transform.rotation, neighbor,
                    neighbor.transform.position, neighbor.transform.rotation, out Vector3 direction,
                    out float distance))
            {
                transform.position += direction * distance;
            }

            ;
        }
    }

    private void HandleGust()
    {
        if (Machine.IsInState(GravityFsmState.DontApplyGustYVelocity)) return;
        var neighbors = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * 3f, 0.5f,
            LayerMask.GetMask("Gust"), QueryTriggerInteraction.Collide);
        foreach (var neighbor in neighbors)
        {
            IsInGust = true;
            
            // Calculate strengthModifier based on y position of player. Goes from 1 -> 0 as the player
            // approaches the top of the gust trigger.
            var localYToGustCollider = transform.position.y - neighbor.transform.position.y;
            var falloffWindowSize = 20f;
            var gustMaxY = neighbor.transform.lossyScale.y * 0.5f;
            var strengthModifier = Mathf.InverseLerp(gustMaxY, gustMaxY - falloffWindowSize, localYToGustCollider + 5f);
            
            // Calculate the sin-based offset which gives a bobbing effect. Increases in strength as long as the player
            // remains near the top of the gust collider. This is to prevent the bobbing from interfering with player
            // motion when traveling upwards through the gust, giving better consistency.
            if (strengthModifier <= 0.5f) _timeAtTopOfGust += Time.deltaTime;
            else _timeAtTopOfGust = 0;
            var offsetModifier = Mathf.InverseLerp(0, 2f, _timeAtTopOfGust);
            print(offsetModifier);
            var offset = Mathf.Lerp(0, -15f, Mathf.InverseLerp(-1f, 1f, Mathf.Sin(Time.time * 2.5f))) * offsetModifier;
            
            // Set Y velocity as a function of the strength modifier and the offset. Less aggressive
            // when accelerating, more aggressive when decelerating.
            var desiredYVelocity = Mathf.Lerp(0, 60f, strengthModifier) + offset;
            var lerpStrength = YVelocity < desiredYVelocity ? 1f : 3.5f;
            YVelocity = Mathf.Lerp(YVelocity, desiredYVelocity, Time.deltaTime * lerpStrength);
            
            return;
        }

        _timeAtTopOfGust = 0f;
        IsInGust = false;
    }

    


    private void InstantiateSpringCollider()
    {
        var springColliderPrefab = Resources.Load("Prefab/Fsm/GravityFsmSpringCollider") as GameObject;
        var springCollider = Instantiate(springColliderPrefab, transform.position, Quaternion.identity);
        this.springCollider = springCollider.GetComponentInChildren<GravityFsmSpringCollider>();
        this.springCollider.SetOwner(this);
    }

    protected virtual void OnParentTransformChanged(Transform t)
    {
    }

    public float GetSummedYVelocity()
    {
        return YVelocity;
    }
}