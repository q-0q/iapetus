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
        public static int GroundMove;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {

    }
    
    private PlayerInput _playerInput;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _maxMomentum = 10f;
    private float _momentum = 0f;
    [SerializeField] private float _momentumGainRate = 10f;
    [SerializeField] private float _momentumLossRate = 10f; 
    [SerializeField] private float _momentumTurnLoss = 10f; 
    public static event Action<float> OnPlayerMomentumUpdated;
    
    
    // --------- End of subclass Fsm data ------------- //


    public override void SetupMachine()
    {
        base.SetupMachine();

        
        Machine.Configure(PlayerFsmState.GroundMove)
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
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {
            var v2 = GetInputMovementVector2();
            var v3 = new Vector3(v2.x, 0, v2.y);

            if (v2.magnitude > 0.1f)
            {
                _momentum = Mathf.Min(_maxMomentum, _momentum + _momentumGainRate * Time.deltaTime);
            }
            else
            {
                _momentum = Mathf.Max(0, _momentum - _momentumLossRate * Time.deltaTime);
                v3 = transform.forward;
            }

            var momentumWeight = Mathf.InverseLerp(0, _maxMomentum, _momentum);
            
            var angle = Vector3.SignedAngle(v3.normalized, transform.forward.normalized, transform.up);
            var animationDesiredTurnAmount = Mathf.InverseLerp(25f, -25f, angle);
            animationDesiredTurnAmount = Mathf.Lerp(-1, 1, animationDesiredTurnAmount);
            var turnAmount = Animator.GetFloat("TurnAmount") * momentumWeight;
            Animator.SetFloat("TurnAmount", Mathf.Lerp(turnAmount, animationDesiredTurnAmount, Time.deltaTime * 10f));
            Animator.SetLayerWeight(1, Mathf.Abs(turnAmount));
            

            var quaternion = Quaternion.LookRotation(v3.normalized, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, _rotationSpeed * Time.deltaTime * Mathf.Lerp(4, 1, momentumWeight * 5f));
            Animator.SetFloat("Momentum", momentumWeight);
            var value = Mathf.Lerp(0f, 3.5f, momentumWeight);
            Animator.SetFloat("SpeedMod", value);
            transform.position += transform.forward.normalized * (_moveSpeed * value * Time.deltaTime);
            
            var momentumDesiredTurnAmount = Mathf.InverseLerp(170f, -170f, angle);
            momentumDesiredTurnAmount = Mathf.Lerp(-1, 1, momentumDesiredTurnAmount);
            print(momentumDesiredTurnAmount);
            _momentum = Mathf.Max(0, _momentum - (_momentumLossRate * Time.deltaTime *
                                                  Mathf.Abs(momentumDesiredTurnAmount) * momentumWeight * _momentumTurnLoss));

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