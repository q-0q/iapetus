using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    private const float Gravity = 125f;
    private const float Damping = 2f; 
    
    private const float InputPower = 50f; 
    private const float MaxInputAngle = 10f;

    private const int NumSegments = 30;
    private const float SegmentDistance = 1f;
    
    
    private Transform _rotator;
    private float _radius; 
    private Vector2 _angularVelocity; 
    private Vector2 _currentAngles;   
    private Vector3 _worldInput;

    private List<GameObject> segments;
    private int currentEffectiveSegmentCount = 0;

    private void Awake()
    {
        _rotator = transform.Find("Rotator");
    }

    private void Start()
    {
        var segmentPrefab = Resources.Load("Prefab/RopeSwingSegment") as GameObject;
        segments = new List<GameObject>();
        for (int i = 0; i < NumSegments; i++)
        {
            var offset = Vector3.down * SegmentDistance * i;
            var segment = GameObject.Instantiate(segmentPrefab, transform.Find("Physics"));
            segment.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
            segments.Add(segment);

            segment.TryGetComponent(out Rigidbody rigidbody);
            if (i == 0)
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                rigidbody.freezeRotation = true;
            }
            else
            {
                segment.TryGetComponent(out ConfigurableJoint joint);
                joint.connectedBody = segments[i - 1].GetComponent<Rigidbody>();
            }
        }
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


        var segmentIndex = Mathf.Clamp(Mathf.FloorToInt(_radius / SegmentDistance), 0, NumSegments - 1);
        currentEffectiveSegmentCount = NumSegments - segmentIndex;
        segments[0].transform.position = PlayerFsm.Singleton.transform.position;
        for (int i = 0; i < NumSegments; i++)
        {
            segments[i].transform.localPosition = Vector3.down * SegmentDistance * i;
            segments[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            segments[i].SetActive(i < currentEffectiveSegmentCount);
        }
    }

    private void Update()
    {
        UpdateMotion();
        UpdateRopeVisuals();

    }

    private void FixedUpdate()
    {
    }

    private void UpdateRopeVisuals()
    {
        if (DoSwingPhysics())
        {
            
            Rigidbody playerSegmentRb = segments[0].GetComponent<Rigidbody>();
            playerSegmentRb.transform.position = PlayerFsm.Singleton.transform.position;
            playerSegmentRb.transform.rotation = Quaternion.identity;
        }

        else
        {
            segments[0].transform.localPosition = Vector3.zero;
        }
    }

    private bool DoSwingPhysics()
    {
        if (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.RopeSwing)) return false;
        if (PlayerFsm.Singleton.currentRopeSwing != this) return false;
        return true;
    }

    private void UpdateMotion()
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
    
    public Vector3 GetWorldSwingDirection()
    {
        if (_radius <= 0) return _rotator.forward;

        // 1. Get the vector from Pivot to Attach Point
        Vector3 offset = GetWorldspaceAttachPoint() - _rotator.position;

        // 2. Flatten the vector to the XZ plane (top-down view)
        offset.y = 0;


        if (offset.sqrMagnitude < 2)
        {
            return PlayerFsm.Singleton.transform.forward; 
        }
        
        print(offset.sqrMagnitude);

        // 4. Return normalized direction
        return offset.normalized;
    }
    
}