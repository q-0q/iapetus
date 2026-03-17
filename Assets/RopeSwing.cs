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

    public Vector2 GetAngularVelocity()
    {
        return _angularVelocity;
    }
    
    public Vector3 GetSwingVelocity()
    {
        if (_radius <= 0) return Vector3.zero;

        // 1. Convert our 2D angular storage into a 3D Angular Velocity vector (omega)
        // In your setup: 
        // _angularVelocity.x rotates around the local X-axis
        // _angularVelocity.y rotates around the local Z-axis
        Vector3 localOmega = new Vector3(_angularVelocity.x, 0, _angularVelocity.y);
    
        // 2. Transform omega to world space
        Vector3 worldOmega = _rotator.TransformDirection(localOmega);

        // 3. Get the vector from the pivot to the player (the radius vector)
        Vector3 radiusVector = GetWorldspaceAttachPoint() - _rotator.position;

        // 4. Linear Velocity = Angular Velocity x Radius Vector
        // This produces the tangential velocity vector in world space
        Vector3 velocity = Vector3.Cross(worldOmega, radiusVector);

        return velocity;
    }
    
    public Vector3 GetWorldAcceleration()
    {
        if (_radius <= 0) return Vector3.zero;

        // 1. Get the radius vector (Pivot to Player)
        Vector3 radiusVector = GetWorldspaceAttachPoint() - _rotator.position;

        // 2. TANGENTIAL ACCELERATION (The swing force)
        // Use the angular acceleration (accelX/Z) calculated in Update
        float alphaX = -(Gravity / _radius) * Mathf.Sin(_currentAngles.x * Mathf.Deg2Rad);
        float alphaZ = -(Gravity / _radius) * Mathf.Sin(_currentAngles.y * Mathf.Deg2Rad);
    
        Vector3 localAlpha = new Vector3(alphaX, 0, alphaZ);
        Vector3 worldAlpha = _rotator.TransformDirection(localAlpha);
        Vector3 tangentialAccel = Vector3.Cross(worldAlpha, radiusVector);

        // 3. CENTRIPETAL ACCELERATION (The pull toward the center)
        // Formula: w x (w x r) or more simply: -direction * (angularVelocity^2 * radius)
        Vector3 localOmega = new Vector3(_angularVelocity.x, 0, _angularVelocity.y);
        Vector3 worldOmega = _rotator.TransformDirection(localOmega);
        Vector3 centripetalAccel = Vector3.Cross(worldOmega, Vector3.Cross(worldOmega, radiusVector));

        // 4. Combine for total world-space acceleration
        return tangentialAccel + centripetalAccel;
    }
}