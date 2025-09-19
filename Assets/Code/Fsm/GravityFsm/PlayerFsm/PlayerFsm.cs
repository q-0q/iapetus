using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerFsm : GravityFsm
{
    // --------- Boilerplate Fsm code --------- //
    
    void Update()
    {
        OnUpdate();
        FireTriggers();
        
        print(TimeInAir);
    }
    
    protected override void OnStart()
    {
        base.OnStart();
    }

    private void Start()
    {
        // Time.timeScale = 0.1f;
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
        public static int HardTurn;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int Jump;
        public static int HardTurn;
        public static int LowMomentum;
    }
    
    private PlayerInput _playerInput;
    private bool _movementAnimationMirror;
    private InputBuffer _inputBuffer;
    
    [SerializeField] private float _sphereCastRadius = 0.75f;
    [SerializeField] private float _wallDetectionDistance = 0.1f;
    
    
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    public const float MaxMomentum = 15f;
    private float _momentum = 0f;
    [SerializeField] private float _momentumGainRate = 10f;
    [SerializeField] private float _momentumLossRate = 10f; 
    [SerializeField] private float _momentumTurnLoss = 10f; 
    public static event Action<float> OnPlayerMomentumUpdated;
    
    [SerializeField] private float _jumpYVelocity = 10f;
    [SerializeField] private float _coyoteTime = 0.1f;
    
    
    // --------- End of subclass Fsm data ------------- //


    public override void SetupMachine()
    {
        base.SetupMachine();

        
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Jump)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("GroundMove");
            });
        
        Machine.Configure(PlayerFsmState.Jumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jumpsquat");
                _inputBuffer.ConsumeBuffer("Jump");
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
            })
            .OnEntryFrom(FsmTrigger.Timeout,_ =>
            {
                YVelocity = _jumpYVelocity;
            });

        Machine.Configure(PlayerFsmState.HardTurn)
            .Permit(PlayerFsmTrigger.LowMomentum, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Jump)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("HardTurn");
            });

        Machine.Configure(GravityFsmState.Aerial)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= _coyoteTime);
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(PlayerFsmState.Jumpsquat, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.Landsquat, 0.125f);
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
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        _inputBuffer.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        
        
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

            var momentumWeight = Mathf.InverseLerp(0, MaxMomentum, _momentum);
            
            var angle = Vector3.SignedAngle(v3.normalized, transform.forward.normalized, transform.up);
            var animationDesiredTurnAmount = Mathf.InverseLerp(35f, -35f, angle);
            animationDesiredTurnAmount = Mathf.Lerp(-1, 1, animationDesiredTurnAmount);
            var turnAmount = Animator.GetFloat("TurnAmount");
            var turnLerpSpeed = Mathf.Abs(animationDesiredTurnAmount) > Mathf.Abs(turnAmount) ? 10f : 2f;
            Animator.SetFloat("TurnAmount", Mathf.Lerp(turnAmount, animationDesiredTurnAmount, Time.deltaTime * turnLerpSpeed));
            Animator.SetLayerWeight(1, Mathf.Abs(turnAmount) * momentumWeight);
            

            var quaternion = Quaternion.LookRotation(v3.normalized, transform.up);
            var lowMomentumRotationMod = _momentum < 5f ? 4f : 1f;
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime * lowMomentumRotationMod);
            Animator.SetFloat("Momentum", momentumWeight);
            var value = Mathf.Lerp(0f, 3.5f, momentumWeight);
            Animator.SetFloat("SpeedMod", value);
            transform.position += transform.forward.normalized * (_moveSpeed * value * Time.deltaTime);
            
            var momentumDesiredTurnAmount = Mathf.InverseLerp(170f, -170f, angle);
            momentumDesiredTurnAmount = Mathf.Lerp(-1, 1, momentumDesiredTurnAmount);
            _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * Time.deltaTime *
                                                  Mathf.Abs(momentumDesiredTurnAmount) * momentumWeight * _momentumTurnLoss));
        }

        if (Machine.IsInState(GravityFsmState.Aerial) || Machine.IsInState(PlayerFsmState.Jumpsquat) || Machine.IsInState(PlayerFsmState.Landsquat) || Machine.IsInState(PlayerFsmState.HardTurn))
        {
            var momentumWeight = Mathf.InverseLerp(0, MaxMomentum, _momentum);
            var value = Mathf.Lerp(0f, 3.5f, momentumWeight);
            Animator.SetFloat("Momentum", momentumWeight);
            transform.position += transform.forward.normalized * (_moveSpeed * value * Time.deltaTime);
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


    
    // ------------ Helper functions ------------- //
    
    private Vector2 GetInputMovementVector2()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }
}