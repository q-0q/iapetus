using System;
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
        _stateStartPosition = transform.position;
        InitState = PlayerFsmState.GroundMove;
        _movementAnimationMirror = false;
        TryGetComponent(out _playerInput);
        _inputBuffer = new InputBuffer(_playerInput, 0.2f);
        _inputBuffer.InitInput("Jump");
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
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int Jump;
        public static int HardTurn;
        public static int LowMomentum;
        public static int FaceLedge;
        public static int FaceWall;
        public static int FaceOpen;
        public static int VaultDistanceTraveled;
    }
    
    private PlayerInput _playerInput;
    private bool _movementAnimationMirror;
    private InputBuffer _inputBuffer;
    
    private float _forwardRaycastDistance = 1.5f;
    private float _minLedgeHeight = 0.2f;
    private float _maxLedgeHeight = 2f;
    
    private float _moveSpeed = 5f;
    private float _rotationSpeed = 3f;
    public const float MaxMomentum = 15f;
    private float _momentum = 0f;
    private float _momentumGainRate = 9f;
    private float _momentumLossRate = 20f; 
    private float _momentumTurnLoss = 5f; 
    private float _collisionMomentumLossRate = 300f;
    public static event Action<float> OnPlayerMomentumUpdated;
    public static event Action<Vector3, bool> OnPlayerPositionUpdated;
    
    private float _jumpYVelocity = 22f;
    private float _coyoteTime = 0.05f;
    private float _vaultDistance = 3f;
    private Vector3 _stateStartPosition;
    
    
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
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jump");
            });
        
        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= _coyoteTime)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Fall");
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
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true);

        Machine.Configure(PlayerFsmState.Vault)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            // .PermitIf(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall, _ => TimeInCurrentState() >= 0.3f)
            // .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat, _ => TimeInCurrentState() >= 0.3f)
            // .Permit(PlayerFsmTrigger.VaultDistanceTraveled, PlayerFsmState.Fall)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Vault");
                YVelocity = 0;
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(PlayerFsmState.Jumpsquat, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.Landsquat, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.Vault, 0.35f);
    }

    public override void FireTriggers()
    {
        base.FireTriggers();
        
        if (_inputBuffer.IsBuffered("Jump"))
        {
            Machine.Fire(PlayerFsmTrigger.Jump);
        }
        
        var v2 = GetInputMovementVector2();
        var v3 = new Vector3(v2.x, 0, v2.y);
        var angle = Vector3.Angle(v3.normalized, transform.forward.normalized);
        if (angle > 160f && _momentum > 10f)
        {
            Machine.Fire(PlayerFsmTrigger.HardTurn);
        }

        if (_momentum < 0.25f)
        {
            Machine.Fire(PlayerFsmTrigger.LowMomentum);
        }

        if (Physics.Raycast(transform.position + transform.up * _maxLedgeHeight, transform.forward,
                _forwardRaycastDistance))
        {
            Machine.Fire(PlayerFsmTrigger.FaceWall);
        } else if (Physics.Raycast(transform.position + transform.up * _minLedgeHeight, transform.forward, 
                       _forwardRaycastDistance))
        {
            Machine.Fire(PlayerFsmTrigger.FaceLedge);
        }
        else
        {
            Machine.Fire(PlayerFsmTrigger.FaceOpen);
        }

        if (Vector3.Distance(transform.position, _stateStartPosition) > _vaultDistance)
        {
            Machine.Fire(PlayerFsmTrigger.VaultDistanceTraveled);
        }
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        _inputBuffer.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        OnPlayerPositionUpdated?.Invoke(transform.position, Machine.IsInState(GravityFsmState.Grounded));
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {
            var v2 = GetInputMovementVector2();
            var v3 = new Vector3(v2.x, 0, v2.y);

            if (v2.magnitude > 0.1f)
            {
                _momentum = Mathf.Min(MaxMomentum, _momentum + _momentumGainRate * Time.deltaTime);
            }
            else
            {
                _momentum = Mathf.Max(0, _momentum - _momentumLossRate * Time.deltaTime);
                v3 = transform.forward;
            }
            
            HandleTurning(v3);
            HandleCollisionMove();
            
            SetAnimatorMomentum();
            var speedMod = Mathf.Lerp(0f, 3.5f, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);
        }

        if (Machine.IsInState(PlayerFsmState.Vault))
        {
            _momentum = Mathf.Max(_momentum, 6f);
            var momentumWeight = ComputeMomentumWeight();
            Animator.SetFloat("SpeedMod", Mathf.Lerp(0.3f, 1.1f, momentumWeight));
            if (Physics.Raycast(transform.position + transform.up * 3f + transform.forward * 0.2f,
                    -transform.up, out var hit, 3.1f))
            {
                var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * 60f);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            SetAnimatorMomentum();
            transform.position += ComputeDesiredMove();
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
    }

    private void HandleTurning(Vector3 inputVector3)
    {
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
        var lowMomentumRotationMod = _momentum < 5f ? 4f : 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime * lowMomentumRotationMod);
    }


    // ------------ Helper functions ------------- //
    
    private Vector2 GetInputMovementVector2()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }
    
    private Vector3 ComputeCollisionMove(Vector3 desiredMove)
    {
        var output = desiredMove;
        
        // Radius of your character (adjust as needed)
        float radius = 0.25f;
        float castDistance = 0.65f;
        float pushExitPadding = 0.35f;

        Vector3 position = transform.position + transform.up * 0.5f;
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

    private void HandleCollisionMove()
    {
        var desiredMove = ComputeDesiredMove();
        var collisionMove = ComputeCollisionMove(desiredMove);
        transform.position += collisionMove;
            
        var collisionRatio = (desiredMove.magnitude + 1f) / (collisionMove.magnitude + 1f);
        _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * Time.deltaTime * (collisionRatio - 1f) * _collisionMomentumLossRate));
    }
}