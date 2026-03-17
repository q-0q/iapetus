using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float Gravity = 125f;
    [SerializeField] private float Damping = 2f; 
    [SerializeField] private float InputPower = 60f; // Force of the player's push

    private Transform _rotator;
    private float _radius; 
    private Vector2 _angularVelocity; 
    private Vector2 _currentAngles;   
    private Vector3 _worldInput; // Now storing the Worldspace Vector3

    private void Awake()
    {
        _rotator = transform.Find("Rotator");
    }

    /// <summary>
    /// Accepts a world-space direction (e.g., from Camera or Keyboard).
    /// Assumes y = 0 as requested.
    /// </summary>
    public void SetWorldPlayerInput(Vector3 worldDir)
    {
        _worldInput = worldDir;
    }

    public void SetPlayerPosition(Vector3 playerPos)
    {
        _radius = Vector3.Distance(_rotator.position, playerPos);
        _radius = Mathf.Max(_radius, 0.5f); 
    }

    public void SetPlayerMomentum(float momentum)
    {
        momentum *= 2f;
        Vector3 localDir = _rotator.InverseTransformDirection(PlayerFsm.Singleton.transform.forward);
        
        float omegaX = -(localDir.z * momentum) / _radius;
        float omegaZ = (localDir.x * momentum) / _radius;

        _angularVelocity = new Vector2(omegaX, omegaZ);
        
        Vector3 startAngles = _rotator.localEulerAngles;
        _currentAngles.x = NormalizeAngle(startAngles.x);
        _currentAngles.y = NormalizeAngle(startAngles.z);
    }

    private void Update()
    {
        if (_radius <= 0) return;

        // 1. Convert World Input to Local Swing Space
        // We project the world-space input into the rotator's local space
        Vector3 localInput = _rotator.InverseTransformDirection(_worldInput);

        // 2. Calculate Restoring Acceleration (Gravity)
        float accelX = -(Gravity / _radius) * Mathf.Sin(_currentAngles.x * Mathf.Deg2Rad);
        float accelZ = -(Gravity / _radius) * Mathf.Sin(_currentAngles.y * Mathf.Deg2Rad);

        // 3. Add Player Input (Mapping local movement to angular change)
        // Local Z movement (forward/back) creates rotation around Local X axis
        // Local X movement (left/right) creates rotation around Local Z axis
        float playerAccelX = -(localInput.z * InputPower) / _radius;
        float playerAccelZ = (localInput.x * InputPower) / _radius;

        // 4. Update Angular Velocity
        _angularVelocity.x += (accelX + playerAccelX) * Time.deltaTime;
        _angularVelocity.y += (accelZ + playerAccelZ) * Time.deltaTime;

        // 5. Apply Damping
        _angularVelocity -= _angularVelocity * Damping * Time.deltaTime;

        // 6. Update the Angles
        _currentAngles.x += _angularVelocity.x * Mathf.Rad2Deg * Time.deltaTime;
        _currentAngles.y += _angularVelocity.y * Mathf.Rad2Deg * Time.deltaTime;

        // 7. Apply to Transform
        _rotator.localRotation = Quaternion.Euler(_currentAngles.x, 0, _currentAngles.y);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }

    public Vector3 GetWorldspaceAttachPoint()
    {
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