using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    [Header("Settings")]
    private const float Gravity = 125f;
    private const float Damping = 2f; // Velocity loss over time

    private Transform _rotator;
    private float _radius; 
    private Vector2 _angularVelocity; // X is swing around X-axis, Y is swing around Z-axis
    private Vector2 _currentAngles;   // Current X and Z rotation values

    private void Awake()
    {
        _rotator = transform.Find("Rotator");
    }

    public void SetPlayerPosition(Vector3 playerPos)
    {
        _radius = Vector3.Distance(_rotator.position, playerPos);
        _radius = Mathf.Max(_radius, 0.5f); // Prevent division by zero
    }
    public void SetPlayerMomentum(float momentum)
    {
        momentum *= 2f;
        
        // 1. Calculate the radius (L) from the pivot

        // 2. Convert player's world forward momentum into local angular velocity
        // We project the player's forward direction into the rotator's local space
        Vector3 localDir = _rotator.InverseTransformDirection(PlayerFsm.Singleton.transform.forward);
        
        // Linear velocity v = angular velocity w * radius -> w = v / r
        // Note: Moving forward (Z) creates rotation around the X-axis
        // Moving right (X) creates rotation around the Z-axis
        float omegaX = -(localDir.z * momentum) / _radius;
        float omegaZ = (localDir.x * momentum) / _radius;

        _angularVelocity = new Vector2(omegaX, omegaZ);
        
        // 3. Initialize current angles based on current rotation
        Vector3 startAngles = _rotator.localEulerAngles;
        _currentAngles.x = NormalizeAngle(startAngles.x);
        _currentAngles.y = NormalizeAngle(startAngles.z);
    }

    private void Update()
    {
        if (_radius <= 0) return;

        // 1. Calculate Restoring Acceleration (Gravity)
        // a = -(g/L) * sin(theta)
        float accelX = -(Gravity / _radius) * Mathf.Sin(_currentAngles.x * Mathf.Deg2Rad);
        float accelZ = -(Gravity / _radius) * Mathf.Sin(_currentAngles.y * Mathf.Deg2Rad);

        // 2. Update Angular Velocity
        _angularVelocity.x += accelX * Time.deltaTime;
        _angularVelocity.y += accelZ * Time.deltaTime;

        // 3. Apply Damping (Linear Drag)
        _angularVelocity -= _angularVelocity * Damping * Time.deltaTime;

        // 4. Update the Angles
        _currentAngles.x += _angularVelocity.x * Mathf.Rad2Deg * Time.deltaTime;
        _currentAngles.y += _angularVelocity.y * Mathf.Rad2Deg * Time.deltaTime;

        // 5. Apply to Transform
        _rotator.localRotation = Quaternion.Euler(_currentAngles.x, 0, _currentAngles.y);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }

    public Vector3 GetWorldspaceAttachPoint()
    {
        // Returns the point at the bottom of the rope based on current rotation
        return _rotator.position + (_rotator.up * -_radius);
    }
}