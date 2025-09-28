using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponFsm : Fsm
{

    public float IdleOrbitRadius = 3f;
    public float IdleOrbitHeight = 3.5f;
    private const float IdlePositionLerpStrength = 3f;
    private const float IdleRotationLerpStrength = 15f;
    
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
        transform.SetParent(null);
        OnStart();
    }

    private void Update()
    {
        OnUpdate();

        if (Machine.IsInState(PlayerWeaponFsmState.Idle))
        {
            var playerPosition = PlayerFsm.Singleton.transform.position;
            var toPlayer = playerPosition - new Vector3(transform.position.x, playerPosition.y, transform.position.z);
            var destinationPos = (playerPosition - toPlayer.normalized * IdleOrbitRadius) + Vector3.up * IdleOrbitHeight;
            transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * IdlePositionLerpStrength);

            var destinationRot = Quaternion.LookRotation((playerPosition + Vector3.up * IdleOrbitHeight) - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRot, Time.deltaTime * IdleRotationLerpStrength);
        }
    }
}