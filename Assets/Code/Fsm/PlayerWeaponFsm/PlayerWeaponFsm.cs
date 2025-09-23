using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponFsm : Fsm
{
    public class PlayerWeaponFsmState : FsmState
    {
        public static int Idle;
    }

    public class PlayerWeaponFsmTrigger : FsmTrigger
    {
        
    }

    public override void SetupMachine()
    {
        base.SetupMachine();

    }
    
    protected override void OnStart()
    {
        base.OnStart();

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


    }

    private void Start()
    {
        InitState = PlayerWeaponFsmState.Idle;
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }
}