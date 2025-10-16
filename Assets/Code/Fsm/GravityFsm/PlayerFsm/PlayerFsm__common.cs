using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using Wasp;

public partial class PlayerFsm
{
    private enum FlankType
    {
        Left,
        Right,
        None
    }

    private PlayerInput _playerInput;
    private InputBuffer _inputBuffer;
    private Camera _camera;
    private float _momentum = 0f;
    private float _stateEntryMomentum = 0f;
    private Vector3 _currentLedgePosition;
    private Vector3 _currentFlankWallNormal;
    private FlankType _currentFlankType;
    private FlankType _previousWallrunSide;
    private Vector3 _checkpointVector3;
    private Quaternion _checkpointQuaternion;
    private Vector3 _walkToPositionTarget;
    private InteractionCollider _currentInteractionCollider;
    private List<ParticleSystem> _kiIndicatorParticles;
    private Material _material;
    private float _timeSinceDashFinished = 0f;

    private bool _movementAnimationMirror;
    private bool _wallsquattedSinceLeavingGround;
    public static PlayerFsm Singleton;
    
    public static event Action<float> OnPlayerMomentumUpdated;
    public static event Action<Vector3, bool> OnPlayerPositionUpdated;
    public static event Action OnPlayerImpaleStateEntered;
    public static event Action OnPlayerGrappleStateEntered;
    public static event Action OnPlayerRacePressed;
    
    public const float InputMagnitudeThreshhold = 0.1f;
    private const float InteractionDistance = 2.5f;
    
    private const float ForwardRaycastDistance = 0.9f;
    private const float DynamicForwardRaycastMaximumModifier = 2f;
    // private const float CollisionMoveSphereCastRadius = 0.4f;
    // private const float GroundCollisionMoveSphereCastHeight = 0.95f;
    // private const float FallingCollisionMoveSphereCastHeight = -0.5f;
    // private const float FallingCollisionMoveSphereCastHeightYVelocityThreshhold = -10f;
    // private const float CollisionMoveSphereCastDistance = 0.45f;
    private const float FaceLedgeHeight = 0.2f;
    private const float FaceHighLedgeHeight = 2.15f;
    private const float FaceWallHeight = 2.4f;
    private const float FaceWallMaximumAngle = 60f;
    private const float FaceWallStrictMaximumAngle = 20f;
    private const float MaximumFlankWallDistance = 7.5f;
    private const float FlankWallHeight = 3f;
    private const float FlankWallOpenYOffset = -2f;
    private const float FlankMaximumAngle = 40f;
    private const float ForceWallRotationRaycastDistance = 3f;

    public const float MaxMomentum = 15f;
    private const float MoveSpeed = 5f;
    private const float MaximumMomentumSpeedMod = 3.5f;
    private const float RotationSpeed = 3f;
    private const float CollisionMomentumLossRate = 300f;
    private const float MomentumGainRate = 12f;
    private const float MomentumLossRate = 20f;
    private const float MomentumTurnLoss = 5f;
    private const float NoMomentumThreshold = 0.25f;
    private const float LowMomentumThreshhold = 4.75f;
    private const float LowMomentumRotationMod = 3f;
    private const float LowMomentumMomentumGainMod = 1.15f;
    private const float LowMomentumMomentumLossMod = 1.25f;
    private const float GroundMoveMinimumAnimatorSpeedMod = 0.45f;
    private const float GroundMoveMaximumAnimatorSpeedMod = 3.5f;
    private const float GroundSlopeMaximumMomentumAngle = 120f;
    private const float GroundSlopeMaximumMomentumModifier = 0.55f;
    
    private const float JumpYVelocity = 22f; 
    private const float CoyoteTime = 0.04f;
    private const float AirControlTurningMultiplier = 0.8f;
    private const float AirControlTurningMomentumDecayModifier = 0.15f;
    private const float AirControlMomentumDecayModifier = 0.35f;
    
    private const float UpdateLedgePositionEpsilon = 3f;
    private const float VaultMinimumYVelocity = -2f;
    private const float VaultMinimumMomentum = 6f;
    private const float VaultMinimumMomentumOnUpdate = 6f;
    private const float VaultHangLedgeYOffset = -2.5f;
    private const float VaultHangLedgeLerpStrength = 60f;
    private const float VaultTurningMultiplier = 0.75f;
    private const float VaultMinimumAnimatorSpeedMod = 0.5f;
    private const float VaultMaximumAnimatorSpeedMod = 1.1f;
    private const float VaultLedgeLerpStrength = 40f;
    private const float MediumVaultHangMinimumYVelocity = 12f;
    private const float SlowVaultFinishLedgeLerpStrength = 25f;
    private const float SlowVaultFinishForwardSpeed = 2f;
    private const float WallsquatMinimumYVelocity = 10f;
    private const float WallSquatMinimumMomentum = 0f;
    private const float WallstepMinimumYVelocityGain = 12f;
    private const float WallstepMaximumYVelocityGain = 23.5f;
    private const float WallstepMinimumDuration = 0.25f;
    private const float ForceWallRotationSpeed = 3f;
    private const float WallRunMinimumEntryMomentum = 9f;
    private const float WallRunMinimumMomentum = 9f;
    private const float
        WallRunMinimumYVelocity = 13f; // It's pretty important that this value is larger than WallsquatMinimumYVelocity

    private const float FlankAlignmentRotationSpeed = 25f;
    private const float FlankWallVacuumStrength = 20f;
    private const float WallrunJumpAngle = 75f;
    
    private const float HardLandAirDiff = -9;
    private const float HardLandExitMomentum = 4f;
    private const float HardLandForwardSpeed = 10f;
    private const float HardLandForwardDuration = 0.105f;
    
    private const float HardLandRollExitMomentum = 12f;
    private const float HardLandRollMinimumMomentum = 7.5f;
    private const float HardLandRollForwardSpeed = 15f;
    
    private const float HardTurnMinimumAngle = 130f;
    private const float HardTurnMinimumMomentum = 8.5f;
    private const float HardTurnMomentumLossModifier = 1.25f;
    
    private const float DashEntryMomentumGain = 5f;
    private const float DashEntryMinimumMomentum = 12f;
    private const float DashsquatTurnMultiplier = 2.25f;
    private const float DashForwardSpeed = 18f;
    private const float DashRaycastHeightOffset = 2f;
    private const float SkipWindowDuration = 0.3f;
    
    
    private const float ImpaleMovementModifier = 1f;
    private const float ImpaleMomentumOffset = 2.5f;
    private const float ImpaleMinimumMomentumAfterOffset = 6f;
    private const float ImpaleMomentumLerpStrenth = 10f;
    
    private const float GrappleStartupRotationLerpStrength = 13f;
    private const float GrappleStartupYPositionLerpStrength = 3f;
    private const float GrappleStartupYPositionOffset = 1f;
    private const float GrappleStartupMomentumLossMod = 1.25f;

    private const float WalkToPositionTurnPhaseAngle = 30f;
    private const float WalkToPositionMomentum = 6f;
    private const float WalkToPositionMomentumLerpStrength = 9f;
    private const float ArriveAtWalkPositionTargetDistance = 1.5f;

    private const float KiMomentumThreshhold = 11.5f;
    

    private bool IsHitValidFlank(RaycastHit hit, bool left)
    {
        float DistanceFromPointToPlane(Vector3 pointA, Vector3 pointB, Vector3 planeNormal)
        {
            Vector3 AB = pointA - pointB;
            float distance = Vector3.Dot(AB, planeNormal.normalized);
            return Mathf.Abs(distance); // Absolute value to get the unsigned distance
        }
        
        float flipMod = left ? -1f : 1f;
        var distance = DistanceFromPointToPlane(transform.position, hit.point, hit.normal);
        var angle = Vector3.Angle(transform.right * flipMod, hit.normal);
        
        return distance < 2.5f && angle < FlankMaximumAngle;;
    }

    private float ComputeDynamicForwardRaycastDistance()
    {
        return Mathf.Lerp(1f, DynamicForwardRaycastMaximumModifier, ComputeMomentumWeight()) * ForwardRaycastDistance * GetRaycastTimeModifier();
    }
    
    
    private void MoveYOntoLedge(float yOffset, float lerpStrength)
    {
        var newY = lerpStrength < 0 ? _currentLedgePosition.y : Mathf.Lerp(transform.position.y, _currentLedgePosition.y + yOffset, Time.deltaTime * lerpStrength);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private bool UpdateLedgePosition(float ledgeHeight)
    {
        var downwardRaycastOrigin = transform.position + (Vector3.up * (ledgeHeight + UpdateLedgePositionEpsilon)) + transform.forward * ComputeDynamicForwardRaycastDistance();
        Debug.DrawLine(downwardRaycastOrigin, downwardRaycastOrigin - (Vector3.up * (ledgeHeight + UpdateLedgePositionEpsilon)), Color.green);

        if (!Physics.Raycast(downwardRaycastOrigin, -Vector3.up, out var hit,
                (ledgeHeight + UpdateLedgePositionEpsilon) * GetRaycastTimeModifier(), GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore)) return false;
        _currentLedgePosition = hit.point;
        return true;
    }
    


    private void HandleFlankAlignment()
    {
        if (_currentFlankType == FlankType.None) return; 
        var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
        var forward = Quaternion.Euler(0f, 90f * rotationMod, 0f) * _currentFlankWallNormal;
        Debug.DrawRay(transform.position, forward, Color.yellow);
        var lookRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * FlankAlignmentRotationSpeed);
    }

    private void HandleTurning(float multiplier = 1f, bool forceForwardInput = false,
        float momentumDecayMultiplier = 1f)
    {
        
        var v3 = GetInputMovementVector3();
        var inputVector3 = forceForwardInput ? MirrorInputForward(v3, transform.forward) : v3;
        
        var v2 = GetInputMovementVector2();
        if (v2.magnitude < InputMagnitudeThreshhold)
        {
            inputVector3 = transform.forward;
        }
        
        HandleTurningCore(multiplier, momentumDecayMultiplier, inputVector3);
    }

    private void HandleTurningCore(float multiplier, float momentumDecayMultiplier, Vector3 direction)
    {
        float momentumWeight = ComputeMomentumWeight();
        var angle = Vector3.SignedAngle(direction.normalized, transform.forward.normalized, Vector3.up);
        var animationDesiredTurnAmount = Mathf.InverseLerp(40f, -40f, angle);
        animationDesiredTurnAmount = Mathf.Lerp(-1, 1, animationDesiredTurnAmount);
        var turnAmount = Animator.GetFloat("TurnAmount");
        var turnLerpSpeed = Mathf.Abs(animationDesiredTurnAmount) > Mathf.Abs(turnAmount) ? 10f : 4.5f;
        Animator.SetFloat("TurnAmount", Mathf.Lerp(turnAmount, animationDesiredTurnAmount, Time.deltaTime * turnLerpSpeed));
        Animator.SetLayerWeight(1, Mathf.Abs(turnAmount) * momentumWeight);
            
        var momentumDesiredTurnAmount = Mathf.InverseLerp(170f, -170f, angle);
        momentumDesiredTurnAmount = Mathf.Lerp(-1, 1, momentumDesiredTurnAmount);
        _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * Time.deltaTime *
                                              Mathf.Abs(momentumDesiredTurnAmount) * momentumWeight * MomentumTurnLoss * momentumDecayMultiplier));
        
        var quaternion = Quaternion.LookRotation(direction.normalized, Vector3.up);
        
        var lowMomentumRotationMod = _momentum < LowMomentumThreshhold ? LowMomentumRotationMod : 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, RotationSpeed * Time.deltaTime * lowMomentumRotationMod * multiplier);
    }

    private void HandleInputMomentumChange(float increaseMultiplier = 1f, float decreaseMultiplier = 1f)
    {
        var v2 = GetInputMovementVector2();
        if (v2.magnitude > InputMagnitudeThreshhold)
        {
            var lowMomentumMomentumGainMod = _momentum < LowMomentumThreshhold ? LowMomentumMomentumGainMod : 1f;
            var weight = Mathf.InverseLerp(90f, GroundSlopeMaximumMomentumAngle, GroundForwardSlope);
            
            var slopeMaxMomentumMod = Mathf.Lerp(1f, GroundSlopeMaximumMomentumModifier,
                weight);
            _momentum = Mathf.Min(MaxMomentum * slopeMaxMomentumMod, _momentum + MomentumGainRate  * lowMomentumMomentumGainMod * increaseMultiplier * Time.deltaTime);
        }
        else
        {
            var lowMomentumMomentumLossMod = _momentum < LowMomentumThreshhold ? LowMomentumMomentumLossMod : 1f;
            _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * lowMomentumMomentumLossMod * decreaseMultiplier * Time.deltaTime));
        }
    }
    
    
    private Vector2 GetInputMovementVector2()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }
    
    public Vector3 GetInputMovementVector3()
    {
        var v2 = GetInputMovementVector2();
        return Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * new Vector3(v2.x, 0, v2.y);
    }
    
    public Vector3 GetMouseVector3()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 v2 = mouseScreenPosition - screenCenter;
        return Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * new Vector3(v2.x, 0, v2.y).normalized;
    }
    

    
    private float ComputeMomentumWeight()
    {
        return Mathf.InverseLerp(0, MaxMomentum, _momentum);
    }

    private Vector3 ComputeDesiredMove()
    {
        var value = Mathf.Lerp(0f, MaximumMomentumSpeedMod, ComputeMomentumWeight());
        return transform.forward.normalized * (MoveSpeed * value * Time.deltaTime);
    }

    private void SetAnimatorMomentum()
    {
        Animator.SetFloat("Momentum", ComputeMomentumWeight());
    }

    private void HandleCollisionMove(float modifier = 1f)
    {
        var desiredMove = ComputeDesiredMove();
        var collisionMove = ComputeCollisionMove(desiredMove);
        transform.position += collisionMove * modifier;
        
        if (Machine.IsInState(PlayerFsmState.Wallsquat)) return;
        if (Machine.IsInState(PlayerFsmState.SlowVaultHang)) return;
        if (Machine.IsInState(PlayerFsmState.Wallrun)) return;
        if (Machine.IsInState(GravityFsmState.Aerial) && YVelocity > WallsquatMinimumYVelocity - 1f) return;
        
        var collisionRatio = (desiredMove.magnitude + 1f) / (collisionMove.magnitude + 1f);
        _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * Time.deltaTime * (collisionRatio - 1f) * CollisionMomentumLossRate));
    }

    public void InvokeBoost(bool jump, float momentumWeight)
    {
        if (jump) Machine.Fire(PlayerFsmTrigger.Jump);
        _momentum = MaxMomentum * momentumWeight;
    }
    
    public void InvokeCheckpoint(Vector3 position, Quaternion rotation)
    {
        _checkpointVector3 = position;
        _checkpointQuaternion = rotation;
    }
    
    public static Vector3 MirrorInputForward(Vector3 input, Vector3 forward)
    {
        if (input == Vector3.zero)
            return Vector3.zero;

        // Project both vectors onto the XZ plane (ignore vertical component)
        Vector3 inputFlat = new Vector3(input.x, 0f, input.z).normalized;
        Vector3 forwardFlat = new Vector3(forward.x, 0f, forward.z).normalized;

        float dot = Vector3.Dot(inputFlat, forwardFlat);

        if (dot >= 0f)
        {
            // Input is within 90 degrees of Forward — return as-is
            return input;
        }
        else
        {
            // Mirror the input vector across the forward's perpendicular plane
            // First, get the right vector (90° rotation from forward)
            Vector3 right = Vector3.Cross(Vector3.up, forwardFlat).normalized;

            // Project input onto the forward-right basis
            float f = Vector3.Dot(inputFlat, forwardFlat);
            float r = Vector3.Dot(inputFlat, right);

            // Mirror the forward component (flip sign of f)
            float mirroredF = -f;

            // Reconstruct the mirrored vector
            Vector3 mirrored = (mirroredF * forwardFlat) + (r * right);

            // Scale by original input magnitude (preserve intensity)
            return mirrored.normalized * input.magnitude;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_currentLedgePosition, 0.25f);
    }
    
    private bool CanGrapple(TriggerParams? triggerParams)
    {
        return PlayerWeaponFsm.Singleton.Machine.IsInState(PlayerWeaponFsm.PlayerWeaponFsmState.ImpaleStuck);
    }
    
    private bool CanImpale(TriggerParams? triggerParams)
    {
        var machine = PlayerWeaponFsm.Singleton.Machine;
        return machine.IsInState(PlayerWeaponFsm.PlayerWeaponFsmState.Idle) || machine.IsInState(PlayerWeaponFsm.PlayerWeaponFsmState.ImpaleStartup);
    }
    
    private bool CanDash(TriggerParams? triggerParams)
    {
        return true;
    }

    private void OnContactHitboxCollide()
    {
        Machine.Fire(PlayerFsmTrigger.ContactHitboxTrigger);
    }



    private void Reset()
    {
        _parentTransform = null;
        Machine.Jump(PlayerFsmState.GroundMove);
        transform.position = _checkpointVector3;
        transform.rotation = _checkpointQuaternion;
        _momentum = 0;
        YVelocity = 0;
    }
    
    private void HandleKiEffects()
    {

        var on = Machine.IsInState(PlayerFsmState.Dash) || Machine.IsInState(PlayerFsmState.Dashsquat);
        
        foreach (var p in _kiIndicatorParticles)
        {
            if (on && !p.isEmitting) p.Play();
            else if (!on) p.Stop();
        }

        var desiredGlowWeight = on ? 3.5f : 0f;
        var currentGlowWeight =  _material.GetFloat("_GlowWeight");
        var f = on ? 5f : 2f;
        _material.SetFloat("_GlowWeight", Mathf.Lerp(currentGlowWeight, desiredGlowWeight, Time.deltaTime * f));
    }

    private float GetCurrentDashRaycastHeightOffset()
    {
        return Machine.IsInState(PlayerFsmState.Dash) || Machine.IsInState(PlayerFsmState.Skip) ? DashRaycastHeightOffset : 0;
    }
}