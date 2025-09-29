using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Serialization;
using Wasp;

public class PlayerWeaponFsm : Fsm
{

    private Transform _subTransform;
    private Vector3 _impaleActiveTargetPosition;

    private const float IdleOrbitRadius = 3f;
    private const float IdleOrbitHeight = 3.5f;
    private const float IdlePositionLerpStrength = 3f;
    private const float IdleRotationLerpStrength = 5f;
    
    private const float ImpaleStartupOrbitRadius = 1f;
    private const float ImpaleStartupOrbitHeight = 4.5f;
    private const float ImpaleStartupPositionLerpStrength = 27.5f;
    private const float ImpaleStartupRotationLerpStrength = 15f;
    private const float ImpaleStartupPullbackSpeed = 3.5f;
    
    private const float ImpaleActivePositionLerpStrength = 45f;
    private const float ImpaleActiveMaxDistance = 10f;
    
    private const float ImpaleStuckRecoveryPullbackSpeed = 14f;

    public static event Action<Vector3, bool> OnPlayerWeaponPositionUpdated;
    private CinemachineImpulseSource _impulseSource;
    
    
    public class PlayerWeaponFsmState : FsmState
    {
        public static int Idle;
        public static int ImpaleStartup;
        public static int ImpaleActive;
        public static int ImpaleRecovery;
        public static int ImpaleStuck;
        public static int ImpaleStuckRecovery;
    }

    public class PlayerWeaponFsmTrigger : FsmTrigger
    {
        public static int PlayerImpaleStateEntered;
        public static int HitTerrain;
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
                _subTransform.DOShakePosition(0.3f, 0.3f);
                transform.rotation = Quaternion.LookRotation(PlayerFsm.Singleton.transform.forward, Vector3.up);
            });
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleActive)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleRecovery)
            .Permit(PlayerWeaponFsmTrigger.HitTerrain, PlayerWeaponFsmState.ImpaleStuck)
            .OnEntry(_ =>
            {
                _impaleActiveTargetPosition = ComputeImpaleActiveTargetPosition();
                transform.rotation =
                    Quaternion.LookRotation(_impaleActiveTargetPosition - transform.position, Vector3.up);
            });
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleRecovery)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.Idle)
            .OnEntry(_ =>
            {
                transform.DOShakePosition(0.5f, 0.3f);
            });
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleStuck)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleStuckRecovery)
            .OnEntry(_ =>
            {
                // _impulseSource.GenerateImpulse();
                HitstopManager.Singleton.StartHitstop(0.075f);
                transform.DOShakePosition(0.5f, 0.3f);
            });
        
        Machine.Configure(PlayerWeaponFsmState.ImpaleStuckRecovery)
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
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStuck, 0.45f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStuckRecovery, 0.1f);
    }

    public override void FireTriggers()
    {
        base.FireTriggers();
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        OnPlayerWeaponPositionUpdated?.Invoke(transform.position, true);
        
        //Machine.IsInState(PlayerWeaponFsmState.ImpaleActive) || Machine.IsInState(PlayerWeaponFsmState.ImpaleStuck)
        
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
            _subTransform.localPosition = Vector3.Lerp(_subTransform.localPosition, Vector3.zero, Time.deltaTime * ImpaleStartupPullbackSpeed * 5f);
        }
        
        if (Machine.IsInState(PlayerWeaponFsmState.ImpaleActive))
        {
            transform.position += transform.forward * (Time.deltaTime * ImpaleActivePositionLerpStrength *
                                                       (TimeInCurrentState() > 0.195f ? 0.25f : 1f));

            // transform.position = Vector3.Lerp(transform.position, _impaleActiveTargetPosition,
            //     Time.deltaTime * ImpaleActivePositionLerpStrength);
        }

        if (Machine.IsInState(PlayerWeaponFsmState.ImpaleStuckRecovery))
        {
            var pullback = Vector3.forward * (-ImpaleStuckRecoveryPullbackSpeed * Time.deltaTime);
            transform.position += pullback;
        }
        
    }

    private void Start()
    {
        InitState = PlayerWeaponFsmState.Idle;
        transform.SetParent(null);
        _subTransform = transform.GetChild(0);
        TryGetComponent(out _impulseSource);
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
        PlayerWeaponCollider.OnPlayerWeaponCollision += OnWeaponCollision;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerImpaleStateEntered -= OnPlayerImpaleEnter;
        PlayerWeaponCollider.OnPlayerWeaponCollision -= OnWeaponCollision;
    }

    public void OnPlayerImpaleEnter()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered);
    }
    
    private void OnWeaponCollision()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.HitTerrain);
    }

    private Vector3 ComputeImpaleActiveTargetPosition()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, ImpaleActiveMaxDistance,
                LayerMask.GetMask("AimAssist"), QueryTriggerInteraction.Collide))
        {
            return hit.transform.position;
        }

        return transform.position + (transform.forward * ImpaleActiveMaxDistance);
    }
}