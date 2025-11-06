using UnityEngine;
using UnityEngine.Serialization;

public class TightropeController : MonoBehaviour
{

    private const float CheckCapsuleRadius = 0.25f;

    public LineRenderer lineRenderer;
    private CapsuleCollider _capsuleCollider;
    public Transform end;
    private Transform _player;
    
    // Start is called before the first frame update
    void Start()
    {
        end = transform.Find("End");
        TryGetComponent(out lineRenderer);
        transform.Find("Trigger").TryGetComponent(out _capsuleCollider);
        _player = PlayerFsm.Singleton.transform;
        
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.SetPosition(2, end.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(2, end.transform.position);
        
        ConfigureCapsuleBetweenPoints(_capsuleCollider, transform.position, end.transform.position, CheckCapsuleRadius);
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
        var forward = end.transform.position - transform.position;
        return Quaternion.LookRotation(forward, Vector3.up);
    }
    
    float DistanceFromLine()
    {
        Vector3 aboveOffset = (Vector3.Angle(_player.up, ClosestPointOnLine(_player.position) - _player.position) > 110f)
            ? _player.up * 2f
            : Vector3.zero;
        float output = Vector3.Distance(_player.position + aboveOffset, ClosestPointOnLine(_player.position));
        
        return output;

    }
    
    
    public Vector3 ClosestPointOnLine(Vector3 queryPoint)
    {
        // Calculate the direction vector of the line
        Vector3 direction = end.position - transform.position;

        // Calculate the vector from point1 to the query point
        Vector3 toQueryPoint = queryPoint - transform.position;

        // Calculate the parameter t which represents the distance along the line
        float t = Vector3.Dot(toQueryPoint, direction) / Vector3.Dot(direction, direction);

        // If t is less than 0, closest point is point1
        if (t <= 0)
        {
            return transform.position;
        }

        // If t is greater than 1, closest point is point2
        if (t >= 1)
        {
            return end.position;
        }

        // Calculate the closest point using the parameter t
        Vector3 closestPoint = transform.position + t * direction;

        return closestPoint;
    }

    private float GetPlayerAngle()
    {
        return Vector3.Angle(_player.up, end.position - transform.position) - 90f;
    }
    
    // public void StartBounceFromAirTime(float time)
    // {
    //     _playerOn = true;
    //     _spring = Instantiate(Resources.Load("TightropeSpring") as GameObject, transform.parent);
    //     _spring.transform.position = _player.position - _player.up * 0.7f;
    //     _spring.transform.rotation = _player.rotation;
    //     _springComp = _spring.GetComponentInChildren<SpringPlatform>();
    // }
    //
    //
    // public void EndBounce()
    // {
    //     _playerOn = false;
    //     _player.SetParent(transform.parent);
    //     _player.GetComponent<Player>().cameraFollow.SetParent(transform.parent);
    //     Destroy(_spring);
    // }
    
    //
    // public void MovePointTowardsEnd(int end, Transform spring)
    // {
    //     spring.transform.position += (_lineRenderer.GetPosition(end) - spring.transform.position).normalized * Time.deltaTime * 9f;
    // }
    
    private void UpdatePositions()
    {
        Vector3[] positions = { transform.position, lineRenderer.GetPosition(1), end.position };
        //_baseMiddlePoint = (transform.position + _end.position) / 2f;
        //if (!_playerOn) _middlePoint = _baseMiddlePoint;
        lineRenderer.SetPositions(positions);
    }
}
