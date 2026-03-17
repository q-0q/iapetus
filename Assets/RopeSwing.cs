using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    private const float Gravity = 125f;
    private const float Damping = 2f; 
    
    private const float InputPower = 50f; 
    private const float MaxInputAngle = 10f;

    private Transform _rotator;
    private float _radius; 
    private Vector2 _angularVelocity; 
    private Vector2 _currentAngles;   
    
    private Vector3 _worldInput;

    private void Awake()
    {
        _rotator = transform.Find("Rotator");
    }

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
        _angularVelocity = new Vector2(-(localDir.z * momentum) / _radius, (localDir.x * momentum) / _radius);
        
        Vector3 startAngles = _rotator.localEulerAngles;
        _currentAngles.x = NormalizeAngle(startAngles.x);
        _currentAngles.y = NormalizeAngle(startAngles.z);
    }

    private void Update()
    {
        if (_radius <= 0) return;

        // 1. Calculate Gravity Acceleration
        float accelX = -(Gravity / _radius) * Mathf.Sin(_currentAngles.x * Mathf.Deg2Rad);
        float accelZ = -(Gravity / _radius) * Mathf.Sin(_currentAngles.y * Mathf.Deg2Rad);

        // 2. Process Short-Term Player Input
        Vector2 playerAngularAccel = Vector2.zero;
        if (_worldInput.sqrMagnitude > 0.01f)
        {
            Vector3 localInput = _rotator.InverseTransformDirection(_worldInput);
            
            // Map input to angular axes
            float targetAccelX = -(localInput.z * InputPower) / _radius;
            float targetAccelZ = (localInput.x * InputPower) / _radius;

            // ANGLE THRESHOLD CHECK:
            // We only apply input if the rope hasn't reached MaxInputAngle yet,
            // OR if the player is pushing in the opposite direction of the current tilt.
            
            // Handle X-Axis Swing (Forward/Back)
            if (Mathf.Abs(_currentAngles.x) < MaxInputAngle || Mathf.Sign(targetAccelX) != Mathf.Sign(_currentAngles.x))
            {
                playerAngularAccel.x = targetAccelX;
            }

            // Handle Z-Axis Swing (Left/Right)
            if (Mathf.Abs(_currentAngles.y) < MaxInputAngle || Mathf.Sign(targetAccelZ) != Mathf.Sign(_currentAngles.y))
            {
                playerAngularAccel.y = targetAccelZ;
            }
        }

        // 3. Update Angular Velocity
        _angularVelocity.x += (accelX + playerAngularAccel.x) * Time.deltaTime;
        _angularVelocity.y += (accelZ + playerAngularAccel.y) * Time.deltaTime;

        // 4. Damping & Integration
        _angularVelocity -= _angularVelocity * Damping * Time.deltaTime;
        _currentAngles.x += _angularVelocity.x * Mathf.Rad2Deg * Time.deltaTime;
        _currentAngles.y += _angularVelocity.y * Mathf.Rad2Deg * Time.deltaTime;

        _rotator.localRotation = Quaternion.Euler(_currentAngles.x, 0, _currentAngles.y);
    }

    private float NormalizeAngle(float angle) => angle > 180 ? angle - 360 : angle;

    public Vector3 GetWorldspaceAttachPoint() => _rotator.position + (_rotator.up * -_radius);
    
    public Vector3 GetWorldAcceleration()
    {
        if (_radius <= 0) return Vector3.zero;

        Vector3 radiusVector = GetWorldspaceAttachPoint() - _rotator.position;

        float alphaX = -(Gravity / _radius) * Mathf.Sin(_currentAngles.x * Mathf.Deg2Rad);
        float alphaZ = -(Gravity / _radius) * Mathf.Sin(_currentAngles.y * Mathf.Deg2Rad);
    
        Vector3 localAlpha = new Vector3(alphaX, 0, alphaZ);
        Vector3 worldAlpha = _rotator.TransformDirection(localAlpha);
        Vector3 tangentialAccel = Vector3.Cross(worldAlpha, radiusVector);

        Vector3 localOmega = new Vector3(_angularVelocity.x, 0, _angularVelocity.y);
        Vector3 worldOmega = _rotator.TransformDirection(localOmega);
        Vector3 centripetalAccel = Vector3.Cross(worldOmega, Vector3.Cross(worldOmega, radiusVector));

        return tangentialAccel + centripetalAccel;
    }
}