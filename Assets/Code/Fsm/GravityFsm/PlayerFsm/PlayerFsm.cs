using System;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class PlayerFsm : GravityFsm
{
    // --------- Boilerplate Fsm code --------- //
    
    void Update()
    {
        OnUpdate();
        FireTriggers();
        
    }
    
    protected override void OnStart()
    {
        base.OnStart();
    }

    private void Start()
    {
        InitState = PlayerFsmState.GroundMove;
        _movementAnimationMirror = false;
        TryGetComponent(out _playerInput);
        _inputBuffer = new InputBuffer(_playerInput, 0.275f);
        _inputBuffer.InitInput("Jump");
        _inputBuffer.InitInput("Dash");
        _camera = Camera.main;
        
        // QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        // Application.targetFrameRate = 10;
        OnStart();
    }
    
    // --------- End of boilerplate Fsm code --------- //
    
    // --------- Subclass Fsm data ------------- //
    
    public class PlayerFsmState : GravityFsmState
    {
        public static int GroundMove;
        public static int Jumpsquat;
        public static int Landsquat;
        public static int Jump;
        public static int Fall;
        public static int HardTurn;
        public static int Vault;
        public static int Wallstep;
        public static int Wallsquat;
        public static int ForceFaceWallRotation;
        public static int SlowVaultHang;
        public static int MediumVaultHang;
        public static int SlowVaultFinish;
        public static int LongFall;
        public static int LongFallJump;
        public static int Dashsquat;
        public static int Dash;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int Jump;
        public static int HardTurn;
        public static int LowMomentum;
        public static int FaceLedge;
        public static int FaceHighLedge;
        public static int FaceWall;
        public static int FaceOpen;
        public static int StartLongFall;
        public static int Dash;
    }
    
    private PlayerInput _playerInput;
    private bool _movementAnimationMirror;
    private InputBuffer _inputBuffer;
    
    private float _forwardRaycastDistance = 1f;
    private float _faceLedgeHeight = 0.2f;
    private float _faceHighLedgeHeight = 2.15f;
    private float _faceWallHeight = 2.4f;
    private float _minYVelocityToInteractWithWall = 0f; 
                                                        
    // private float _minYVelocityToSlowVault = 1f;
    
    private float _moveSpeed = 5f;
    private float _rotationSpeed = 3f;
    public const float MaxMomentum = 15f;
    private float _momentum = 0f;
    private float _momentumGainRate = 9f;
    private float _momentumLossRate = 20f; 
    private float _momentumTurnLoss = 5f; 
    private float _lowMomentumThreshhold = 4.75f;
    private float _lowMomentumRotationMod = 3f;
    private float _lowMomentumMomentumGainMod = 1.15f;
    private float _lowMomentumMomentumLossMod = 1.25f;
    private float _collisionMomentumLossRate = 300f;
    public static event Action<float> OnPlayerMomentumUpdated;
    public static event Action<Vector3, bool> OnPlayerPositionUpdated;
    
    private float _jumpYVelocity = 22f;
    private float _coyoteTime = 0.05f;
    private float _vaultDistance = 3f;

    private Vector3 _currentLedgePosition;
    private Vector3 _checkpointVector3;
    private Quaternion _checkpointQuaternion;

    private Camera _camera;
    
    
    // --------- End of subclass Fsm data ------------- //


    public override void SetupMachine()
    {
        base.SetupMachine();

        // Machine.OnTransitioned(_ =>
        // {
        //     _stateStartPosition = transform.position;
        // });
        
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("GroundMove");
            });
        
        Machine.Configure(PlayerFsmState.Jumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jumpsquat");
                _inputBuffer.ConsumeBuffer("Jump");
            })
            .OnExitFrom(FsmTrigger.Timeout,_ =>
            {
                YVelocity = _jumpYVelocity;
            });
        
        Machine.Configure(PlayerFsmState.Landsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Landsquat");
            })
            .OnExit(_ =>
            {
                _movementAnimationMirror = !_movementAnimationMirror;
                var flip = _movementAnimationMirror ? 0 : 1f;
                Animator.SetFloat("Flip", flip);
            });
        
        Machine.Configure(PlayerFsmState.Jump)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > -2f)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => _momentum > 3f && YVelocity < _minYVelocityToInteractWithWall)
            .Permit(PlayerFsmTrigger.StartLongFall, PlayerFsmState.LongFallJump)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jump");
            });
        
        Machine.Configure(PlayerFsmState.LongFallJump)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => _momentum > 3f && YVelocity < _minYVelocityToInteractWithWall)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("LongFall");
            });
        
        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= _coyoteTime)
            // .Permit(PlayerFsmTrigger.StartLongFall, PlayerFsmState.LongFall)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Fall");
            });
        
        Machine.Configure(PlayerFsmState.LongFall)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= _coyoteTime)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("LongFall");
            });

        Machine.Configure(PlayerFsmState.HardTurn)
            .Permit(PlayerFsmTrigger.LowMomentum, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("HardTurn");
            });

        Machine.Configure(GravityFsmState.Aerial)
            ;

        Machine.Configure(PlayerFsmState.Vault)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                UpdateLedgePosition(_faceLedgeHeight);
                ReplaceAnimatorTrigger("Vault");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.SlowVaultHang)
            .SubstateOf(PlayerFsmState.ForceFaceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                UpdateLedgePosition(_faceHighLedgeHeight);
                ReplaceAnimatorTrigger("SlowVaultHang");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.MediumVaultHang)
            .SubstateOf(PlayerFsmState.ForceFaceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                UpdateLedgePosition(_faceHighLedgeHeight);
                ReplaceAnimatorTrigger("MediumVaultHang");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceFaceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("SlowVaultFinish");
                YVelocity = 0;
            })
            .OnExit(_ =>
            {
                _momentum = 3f;
            });
        
        Machine.Configure(PlayerFsmState.Wallstep)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceFaceWallRotation)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.SlowVaultHang, _ => YVelocity < 12f)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.MediumVaultHang, _ => YVelocity > 12f, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => YVelocity > 12f, 1)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                ReplaceAnimatorTrigger("Wallstep");
                YVelocity = Mathf.Lerp(12f, 23.5f, ComputeMomentumWeight());
                Animator.SetFloat("VerticalMomentum", ComputeMomentumWeight());
                _momentum = 0;
            });
        
        Machine.Configure(PlayerFsmState.Wallsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceFaceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            // .Permit(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep, _ => TimeInCurrentState() > 0.25f, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                ReplaceAnimatorTrigger("Wallsquat");
            })
            .OnExitFrom(PlayerFsmTrigger.FaceOpen, _ =>
            {
                _momentum = 0;
            });
        
        Machine.Configure(PlayerFsmState.Dashsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Dash)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Dash");
                ReplaceAnimatorTrigger("Dashsquat");
            });
        
        Machine.Configure(PlayerFsmState.Dash)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => true)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                _momentum = Mathf.Min(Mathf.Max(_momentum + 5f, 12f), MaxMomentum);
                ReplaceAnimatorTrigger("Dash");
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(PlayerFsmState.Jumpsquat, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.Landsquat, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.Vault, 0.25f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultHang, 0.975f);
        StateMapConfig.Duration.Add(PlayerFsmState.MediumVaultHang, 0.375f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultFinish, 0.3f);
        StateMapConfig.Duration.Add(PlayerFsmState.Wallsquat, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dashsquat, 0.1f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dash, 0.15f);
        
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallstep, 0.5f);
    }

    public override void FireTriggers()
    {
        base.FireTriggers();
        
        if (_inputBuffer.IsBuffered("Jump"))
        {
            Machine.Fire(PlayerFsmTrigger.Jump);
        }
        
        // if (_inputBuffer.IsBuffered("Dash"))
        // {
        //     Machine.Fire(PlayerFsmTrigger.Dash);
        // }
        
        var v3 = GetInputMovementVector3();
        var angle = Vector3.Angle(v3.normalized, transform.forward.normalized);
        if (angle > 130f && _momentum > 8.5f)
        {
            Machine.Fire(PlayerFsmTrigger.HardTurn);
        }

        if (_momentum < 0.25f)
        {
            Machine.Fire(PlayerFsmTrigger.LowMomentum);
        }

        var distance = ComputeDynamicForwardRaycastDistance();
        if (Physics.Raycast(transform.position + transform.up * _faceWallHeight, transform.forward,
                distance, ~0, QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FaceWall);
        } else if (Physics.Raycast(transform.position + transform.up * _faceHighLedgeHeight, transform.forward, 
                       distance, ~0, QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FaceHighLedge);
        } else if (Physics.Raycast(transform.position + transform.up * _faceLedgeHeight, transform.forward, 
                       out var hit, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, transform.up);
            if (slope > 70f) Machine.Fire(PlayerFsmTrigger.FaceLedge);
        }
        else
        {
            Machine.Fire(PlayerFsmTrigger.FaceOpen);
        }
        
        Debug.DrawRay(transform.position + transform.up * _faceWallHeight, transform.forward * distance, Color.red);
        Debug.DrawRay(transform.position + transform.up * _faceHighLedgeHeight, transform.forward * distance, Color.yellow);
        Debug.DrawRay(transform.position + transform.up * _faceLedgeHeight, transform.forward * distance, Color.cyan);
        
        // if (!Physics.Raycast(transform.position, -transform.up,8f, ~0, QueryTriggerInteraction.Ignore))
        // {
        //     if (YVelocity <= 6) Machine.Fire(PlayerFsmTrigger.StartLongFall);
        // }
        
    }

    private float ComputeDynamicForwardRaycastDistance()
    {
        return Mathf.Lerp(1f, 2f, ComputeMomentumWeight()) * _forwardRaycastDistance * GetRaycastTimeModifier();
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        _inputBuffer.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        OnPlayerPositionUpdated?.Invoke(transform.position, Machine.IsInState(GravityFsmState.Grounded) ||
                                                            Machine.IsInState(PlayerFsmState.ForceFaceWallRotation));
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {

            
            HandleTurning();
            HandleCollisionMove();
            
            SetAnimatorMomentum();
            var speedMod = Mathf.Lerp(0f, 3.5f, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);
        }

        if (Machine.IsInState(PlayerFsmState.SlowVaultHang))
        {
            MoveYOntoLedge(-2.5f, 60f);
            HandleCollisionMove();
            
        }
        if (Machine.IsInState(PlayerFsmState.SlowVaultFinish))
        {
            HandleTurning(0.75f, true);
            MoveYOntoLedge(0, 25f);
            transform.position += transform.forward * (2f * Time.deltaTime);
        }
        else if (Machine.IsInState(PlayerFsmState.Vault))
        {
            _momentum = Mathf.Max(_momentum, 6f);
            var momentumWeight = ComputeMomentumWeight();
            Animator.SetFloat("SpeedMod", Mathf.Lerp(0.3f, 1.1f, momentumWeight));
            MoveYOntoLedge(0f, 40f);
            SetAnimatorMomentum();
            transform.position += ComputeDesiredMove();
            HandleTurning(0.75f, true);
        }
        else if (Machine.IsInState(GravityFsmState.Aerial) || Machine.IsInState(PlayerFsmState.Jumpsquat) || Machine.IsInState(PlayerFsmState.Landsquat) || Machine.IsInState(PlayerFsmState.HardTurn))
        {
            SetAnimatorMomentum();
            HandleCollisionMove();
        }

        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            Animator.SetLayerWeight(1, 0);
        }

        if (Machine.IsInState(PlayerFsmState.HardTurn))
        {
            _momentum = Mathf.Max(0, _momentum - _momentumLossRate * Time.deltaTime * 1.25f);
        }



        if (Machine.IsInState(PlayerFsmState.ForceFaceWallRotation))
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 3f * GetRaycastTimeModifier(), ~0, QueryTriggerInteraction.Ignore))
            {
                var quaternion = Quaternion.LookRotation(-hit.normal, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime * 2f);
            }
        }

        if (Machine.IsInState(PlayerFsmState.Dashsquat))
        {
            HandleCollisionMove();
            HandleTurning(2.25f, true);
        }
        if (Machine.IsInState(PlayerFsmState.Dash))
        {
            Animator.SetLayerWeight(1, 0);
            var collisionMove = ComputeCollisionMove(transform.forward * (20f * Time.deltaTime));
            transform.position += collisionMove;
        }


        if (_playerInput.actions["Reset"].WasPerformedThisFrame())
        {
            transform.position = _checkpointVector3;
            transform.rotation = _checkpointQuaternion;
            _momentum = 0;
            YVelocity = 0;
        }
    }

    private void MoveYOntoLedge(float yTransform, float lerpStrength)
    {
        var newY = lerpStrength < 0 ? _currentLedgePosition.y : Mathf.Lerp(transform.position.y, _currentLedgePosition.y + yTransform, Time.deltaTime * lerpStrength);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void UpdateLedgePosition(float ledgeHeight)
    {
        var f = 3f;
        var downwardRaycastOrigin = transform.position + (transform.up * (ledgeHeight + f)) + transform.forward * ComputeDynamicForwardRaycastDistance();
        Debug.DrawLine(downwardRaycastOrigin, downwardRaycastOrigin - (transform.up * (ledgeHeight + f)), Color.green);
        
        if (Physics.Raycast(downwardRaycastOrigin, -transform.up, out var hit, (ledgeHeight + f) * GetRaycastTimeModifier(), ~0, QueryTriggerInteraction.Ignore))
        {
            _currentLedgePosition = hit.point;
        }
    }

    private void HandleTurning(float multiplier = 1f, bool forceForwardInput = false)
    {
        
        var v2 = GetInputMovementVector2();
        var v3 = GetInputMovementVector3();
        var inputVector3 = forceForwardInput ? MirrorInputForward(v3, transform.forward) : v3;

        if (v2.magnitude > 0.1f)
        {
            var lowMomentumMomentumGainMod = _momentum < _lowMomentumThreshhold ? _lowMomentumMomentumGainMod : 1f;
            _momentum = Mathf.Min(MaxMomentum, _momentum + _momentumGainRate  * lowMomentumMomentumGainMod *  Time.deltaTime);
        }
        else
        {
            var lowMomentumMomentumLossMod = _momentum < _lowMomentumThreshhold ? _lowMomentumMomentumLossMod : 1f;
            _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * lowMomentumMomentumLossMod * Time.deltaTime));
            inputVector3 = transform.forward;
        }
        
        float momentumWeight = ComputeMomentumWeight();
        var angle = Vector3.SignedAngle(inputVector3.normalized, transform.forward.normalized, transform.up);
        var animationDesiredTurnAmount = Mathf.InverseLerp(35f, -35f, angle);
        animationDesiredTurnAmount = Mathf.Lerp(-1, 1, animationDesiredTurnAmount);
        var turnAmount = Animator.GetFloat("TurnAmount");
        var turnLerpSpeed = Mathf.Abs(animationDesiredTurnAmount) > Mathf.Abs(turnAmount) ? 10f : 2f;
        Animator.SetFloat("TurnAmount", Mathf.Lerp(turnAmount, animationDesiredTurnAmount, Time.deltaTime * turnLerpSpeed));
        Animator.SetLayerWeight(1, Mathf.Abs(turnAmount) * momentumWeight);
            
        var momentumDesiredTurnAmount = Mathf.InverseLerp(170f, -170f, angle);
        momentumDesiredTurnAmount = Mathf.Lerp(-1, 1, momentumDesiredTurnAmount);
        _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * Time.deltaTime *
                                              Mathf.Abs(momentumDesiredTurnAmount) * momentumWeight * _momentumTurnLoss));
        
        var quaternion = Quaternion.LookRotation(inputVector3.normalized, transform.up);
        
        var lowMomentumRotationMod = _momentum < _lowMomentumThreshhold ? _lowMomentumRotationMod : 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime * lowMomentumRotationMod * multiplier);
    }


    // ------------ Helper functions ------------- //
    
    private Vector2 GetInputMovementVector2()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }
    
    private Vector3 GetInputMovementVector3()
    {
        var v2 = GetInputMovementVector2();
        return Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * new Vector3(v2.x, 0, v2.y);
    }
    
    private Vector3 ComputeCollisionMove(Vector3 desiredMove)
    {
        var output = desiredMove;
        
        // Radius of your character (adjust as needed)
        float radius = 0.1f;
        float castDistance = 0.45f * GetRaycastTimeModifier();
        float pushExitPadding = 0.35f;

        Vector3 position = transform.position + transform.up * 0.75f;
        Vector3 direction = output.normalized;

        // SphereCast to account for player volume
        if (Physics.SphereCast(position, radius, direction, out RaycastHit hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            
            // First collision: slide along the surface
            Vector3 firstNormal = hit.normal;
            output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(firstNormal, transform.up));

            bool corner = false;

            // Cast again in the new direction to handle corner (second surface)
            if (Physics.SphereCast(position, radius, output.normalized, out RaycastHit secondHit, output.magnitude))
            {
                corner = true;
                Vector3 secondNormal = secondHit.normal;

                // Slide again
                output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(secondNormal, transform.up));
                
                if (output.magnitude < 0.01f)
                {
                    output = Vector3.zero;
                }

            }
            
            
        }
        
        return output;
    }
    
    private float ComputeMomentumWeight()
    {
        return Mathf.InverseLerp(0, MaxMomentum, _momentum);
    }

    private Vector3 ComputeDesiredMove()
    {
        var value = Mathf.Lerp(0f, 3.5f, ComputeMomentumWeight());
        return transform.forward.normalized * (_moveSpeed * value * Time.deltaTime);
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
        if (Machine.IsInState(GravityFsmState.Aerial) && YVelocity >_minYVelocityToInteractWithWall - 1f) return;
        
        var collisionRatio = (desiredMove.magnitude + 1f) / (collisionMove.magnitude + 1f);
        _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * Time.deltaTime * (collisionRatio - 1f) * _collisionMomentumLossRate));
    }

    public void InvokeBoost(bool jump)
    {
        if (jump) Machine.Fire(PlayerFsmTrigger.Jump);
        _momentum = MaxMomentum;
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
}