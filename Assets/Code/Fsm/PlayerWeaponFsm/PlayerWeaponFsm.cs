using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponFsm : Fsm
{

    private Transform _subTransform;

    private const float IdleOrbitRadius = 3f;
    private const float IdleOrbitHeight = 3.5f;
    private const float IdlePositionLerpStrength = 3f;
    private const float IdleRotationLerpStrength = 5f;
    
    private const float ImpaleStartupOrbitRadius = 1f;
    private const float ImpaleStartupOrbitHeight = 3.5f;
    private const float ImpaleStartupPositionLerpStrength = 27.5f;
    private const float ImpaleStartupRotationLerpStrength = 25f;
    private const float ImpaleStartupPullbackSpeed = 3.5f;
    
    private const float ImpaleActiveForwardSpeed = 47.5f;
    
    
    public class PlayerWeaponFsmState : FsmState
    {
        public static int Idle;
        public static int ImpaleStartup;
        public static int ImpaleActive;
        public static int ImpaleRecovery;
    }

    public class PlayerWeaponFsmTrigger : FsmTrigger
    {
        public static int PlayerImpaleStateEntered;
    }

    public override void SetupMachine()
    {
        base.SetupMachine();
        
        Machine.Configure(PlayerWeaponFsmState.Idle)
            .Permit(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered, PlayerWeaponFsmState.ImpaleStartup);
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleStartup)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleActive)
            .OnEntry(_ =>
            {
                transform.rotation = Quaternion.LookRotation(PlayerFsm.Singleton.transform.forward, Vector3.up);
            });
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleActive)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleRecovery);
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleRecovery)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.Idle);

    }
    
    protected override void OnStart()
    {
        base.OnStart();

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStartup, 0.35f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleActive, 0.25f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleRecovery, 0.25f);
    }

    public override void FireTriggers()
    {
        base.FireTriggers();
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Machine.IsInState(PlayerWeaponFsmState.Idle))
        {
            var playerPosition = PlayerFsm.Singleton.transform.position;
            var toPlayer = playerPosition - new Vector3(transform.position.x, playerPosition.y, transform.position.z);
            var destinationPos = (playerPosition - toPlayer.normalized * IdleOrbitRadius) + Vector3.up * IdleOrbitHeight;
            transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * IdlePositionLerpStrength);

            var destinationRot = Quaternion.LookRotation((playerPosition + Vector3.up * IdleOrbitHeight) - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRot, Time.deltaTime * IdleRotationLerpStrength);
        }
        
        if (Machine.IsInState(PlayerWeaponFsmState.ImpaleStartup))
        {
            var playerPosition = PlayerFsm.Singleton.transform.position;
            var pullback = Vector3.forward * (-ImpaleStartupPullbackSpeed * Time.deltaTime);
            _subTransform.localPosition += pullback;
            
            var toPlayer = playerPosition - new Vector3(transform.position.x, playerPosition.y, transform.position.z);
            var destinationPos = (playerPosition - toPlayer.normalized * ImpaleStartupOrbitRadius) +
                                 (Vector3.up * ImpaleStartupOrbitHeight);
            transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * ImpaleStartupPositionLerpStrength);

            var forward = PlayerFsm.Singleton.GetInputMovementVector3().normalized;
            if (forward.magnitude < PlayerFsm.InputMagnitudeThreshhold) forward = transform.forward; 
            var destinationRot = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRot, Time.deltaTime * ImpaleStartupRotationLerpStrength);
        }
        else
        {
            _subTransform.localPosition = Vector3.Lerp(_subTransform.localPosition, Vector3.zero, Time.deltaTime * ImpaleStartupPullbackSpeed);
        }
        
        if (Machine.IsInState(PlayerWeaponFsmState.ImpaleActive))
        {
            transform.position += transform.forward * (Time.deltaTime * ImpaleActiveForwardSpeed *
                                                       (TimeInCurrentState() > 0.195f ? 0.25f : 1f));
        }
        
    }

    private void Start()
    {
        InitState = PlayerWeaponFsmState.Idle;
        transform.SetParent(null);
        _subTransform = transform.GetChild(0);
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
        FireTriggers();
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerImpaleStateEntered += OnPlayerImpaleEnter;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerImpaleStateEntered -= OnPlayerImpaleEnter;
    }

    public void OnPlayerImpaleEnter()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered);
    }
}