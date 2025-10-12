using UnityEngine;

public class TightropeController : MonoBehaviour
{

    private const float CheckCapsuleRadius = 0.25f;
    
    private LineRenderer _lineRenderer;
    private CapsuleCollider _capsuleCollider;
    private Transform _end;
    
    // Start is called before the first frame update
    void Start()
    {
        _end = transform.Find("End");
        TryGetComponent(out _lineRenderer);
        transform.Find("Trigger").TryGetComponent(out _capsuleCollider);
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _end.transform.position);
        
        ConfigureCapsuleBetweenPoints(_capsuleCollider, transform.position, _end.transform.position, CheckCapsuleRadius);
    }
    
    public static void ConfigureCapsuleBetweenPoints(CapsuleCollider capsule, Vector3 pointA, Vector3 pointB, float radius)
    {
        Vector3 direction = pointB - pointA;
        float distance = direction.magnitude;

        if (distance <= 0f) return;

        // Set the radius
        capsule.radius = radius;

        // CapsuleCollider height is the total length including the spheres at both ends
        // So it must be at least (2 * radius)
        capsule.height = Mathf.Max(distance, 2f * radius);

        // Calculate midpoint between A and B
        Vector3 midPoint = (pointA + pointB) * 0.5f;

        // Convert worldspace midpoint to local position relative to the capsule's transform
        capsule.transform.position = midPoint;

        // Set capsule direction (0 = X, 1 = Y, 2 = Z). We'll align it along the local Y axis, so rotate the object.
        capsule.direction = 1; // Y axis

        // Rotate the capsule so its local Y axis aligns with the vector from A to B
        capsule.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    public Quaternion GetAlignmentRotation()
    {
        var forward = _end.transform.position - transform.position;
        return Quaternion.LookRotation(forward, Vector3.up);
    }
}
