using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerFsm : GravityFsm
{
    void Update()
    {
        OnUpdate();
        FireTriggers();
    }

    private void Start()
    {
        InitState = PlayerFsmState.Walk;
        print("initState: " + InitState);
        OnStart();
    }
    
    public class PlayerFsmState : GravityFsmState
    {
        public static int Idle;
        public static int Walk;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int InputDirection;
        public static int NoInputDirection;
    }

    private PlayerInput _playerInput;

    public override void SetupMachine()
    {
        base.SetupMachine();
        
        Machine.Configure(PlayerFsmState.Idle)
            .Permit(PlayerFsmTrigger.InputDirection, PlayerFsmState.Walk)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                Animator.ResetTrigger("Walk");
                Animator.SetTrigger("Idle");
            });
        
        Machine.Configure(PlayerFsmState.Walk)
            .Permit(PlayerFsmTrigger.NoInputDirection, PlayerFsmState.Idle)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                Animator.ResetTrigger("Idle");
                Animator.SetTrigger("Walk");
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }

    public override void FireTriggers()
    {
        base.FireTriggers();

        var v = _playerInput.actions["Move"].ReadValue<Vector2>();
        
        Machine.Fire(v.magnitude > 0.1f ? PlayerFsmTrigger.InputDirection : PlayerFsmTrigger.NoInputDirection);

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        // print(Machine.State());
    }

    protected override void OnStart()
    {
        base.OnStart();
        TryGetComponent(out _playerInput);
    }


}