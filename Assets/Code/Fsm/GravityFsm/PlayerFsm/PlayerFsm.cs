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
    }

    private void Start()
    {
        InitState = PlayerFsmState.GroundMove;
        OnStart();
    }
    
    // --------- End of boilerplate Fsm code --------- //
    
    // --------- Subclass Fsm data ------------- //
    
    public class PlayerFsmState : GravityFsmState
    {
        public static int Idle;
        public static int GroundMove;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int InputDirection;
        public static int NoInputDirection;
    }
    
    private PlayerInput _playerInput;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _maxMomentum = 10f;
    private float _momentum = 0f;
    [SerializeField] private float _momentumGainRate = 10f;
    [SerializeField] private float _momentumLossRate = 10f; 
    public static event Action<float> OnPlayerMomentumUpdated;
    
    
    // --------- End of subclass Fsm data ------------- //


    public override void SetupMachine()
    {
        base.SetupMachine();

        
        Machine.Configure(PlayerFsmState.Idle)
            .Permit(PlayerFsmTrigger.InputDirection, PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Idle");
            });
        
        Machine.Configure(PlayerFsmState.GroundMove)
            .Permit(PlayerFsmTrigger.NoInputDirection, PlayerFsmState.Idle)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("GroundMove");
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }

    public override void FireTriggers()
    {
        base.FireTriggers();

        var v = GetInputMovementVector2();
        
        Machine.Fire(v.magnitude > 0.1f ? PlayerFsmTrigger.InputDirection : PlayerFsmTrigger.NoInputDirection);
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        
        if (Machine.IsInState(PlayerFsmState.Idle))
        {
            _momentum = 0;
        }
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {
            var v2 = GetInputMovementVector2();
            var v3 = new Vector3(v2.x, 0, v2.y);
            var quaternion = Quaternion.LookRotation(v3.normalized, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime);
            
            _momentum = Mathf.Min(_maxMomentum, _momentum + _momentumGainRate * Time.deltaTime);

            var inverseLerp = Mathf.InverseLerp(0, _maxMomentum, _momentum);
            Animator.SetFloat("Momentum", inverseLerp);
            var value = Mathf.Lerp(1f, 2f, inverseLerp);
            Animator.SetFloat("SpeedMod", value);
            transform.position += transform.forward.normalized * (_moveSpeed * value * Time.deltaTime);
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        TryGetComponent(out _playerInput);
    }
    
    // ------------ Helper functions ------------- //
    
    private Vector2 GetInputMovementVector2()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }
}