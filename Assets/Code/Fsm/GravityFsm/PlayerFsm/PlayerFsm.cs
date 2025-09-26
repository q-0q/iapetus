using System;
using Unity.Mathematics;
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
        
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 120;
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
        public static int ForceWallRotation;
        public static int SlowVaultHang;
        public static int MediumVaultHang;
        public static int SlowVaultFinish;
        public static int Dashsquat;
        public static int Dash;
        public static int HardLand;
        public static int HardLandRoll;
        public static int Wallrun;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int Jump;
        public static int HardTurn;
        public static int NoMomentum;
        public static int FaceLedge;
        public static int FaceHighLedge;
        public static int FaceWall;
        public static int FaceOpen;
        public static int FlankWall;
        public static int FlankOpen;
        public static int Dash;
    }
    
    // Fluid data
    
    private PlayerInput _playerInput;
    private InputBuffer _inputBuffer;
    private Camera _camera;
    private float _momentum = 0f;
    private Vector3 _currentLedgePosition;
    private Vector3 _currentFlankWallNormal;
    private bool _currentFlankSide;
    private Vector3 _checkpointVector3;
    private Quaternion _checkpointQuaternion;
    private bool _movementAnimationMirror;
    
    // Events
    
    public static event Action<float> OnPlayerMomentumUpdated;
    public static event Action<Vector3, bool> OnPlayerPositionUpdated;
    
    // Input
    
    private const float InputMagnitudeThreshhold = 0.1f;
    
    // Raycasting
    
    private const float ForwardRaycastDistance = 1f;
    private const float DynamicForwardRaycastMaximumModifier = 2f;
    private const float CollisionMoveSphereCastRadius = 0.1f;
    private const float CollisionMoveSphereCastHeight = 0.75f;
    private const float CollisionMoveSphereCastDistance = 0.45f;
    private const float FaceLedgeHeight = 0.2f;
    private const float FaceHighLedgeHeight = 2.15f;
    private const float FaceWallHeight = 2.4f;
    private const float FlankWallDistance = 1f;
    private const float FlankWallHeight = 2.5f;
    private const float FlankMaximumAngle = 40f;
    private const float ForceWallRotationRaycastDistance = 3f;
    
    // General movement
    
    public const float MaxMomentum = 15f;
    private const float MoveSpeed = 5f;
    private const float MaximumMomentumSpeedMod = 3.5f;
    private const float RotationSpeed = 3f;
    private const float CollisionMomentumLossRate = 300f;
    private const float MomentumGainRate = 9f;
    private const float MomentumLossRate = 20f; 
    private const float MomentumTurnLoss = 5f; 
    private const float NoMomentumThreshold = 0.25f;
    private const float LowMomentumThreshhold = 4.75f;
    private const float LowMomentumRotationMod = 3f;
    private const float LowMomentumMomentumGainMod = 1.15f;
    private const float LowMomentumMomentumLossMod = 1.25f;
    private const float GroundMoveMaximumAnimatorSpeedMod = 3.5f;
    
    // Air movement
    
    private const float JumpYVelocity = 22f;
    private const float CoyoteTime = 0.04f;
    
    // Wall interaction
    
    private const float UpdateLedgePositionEpsilon = 3f;
    private const float MinYVelocityToInteractWithWall = 0f; 
    private const float VaultMinimumYVelocity = -2f;
    private const float VaultMinimumMomentum = 6f;
    private const float VaultHangLedgeYOffset = -2.5f;
    private const float VaultHangLedgeLerpStrength = 60f;
    private const float VaultTurningMultiplier = 0.75f;
    private const float VaultMinimumAnimatorSpeedMod = 0.3f;
    private const float VaultMaximumAnimatorSpeedMod = 1.1f;
    private const float VaultLedgeLerpStrength = 40f;
    private const float MediumVaultHangMinimumYVelocity = 12f;
    private const float SlowVaultFinishLedgeLerpStrength = 25f;
    private const float SlowVaultFinishForwardSpeed = 2f;
    private const float WallSquatMinimumMomentum = 3f;
    private const float WallstepMinimumYVelocityGain = 12f;
    private const float WallstepMaximumYVelocityGain = 23.5f;
    private const float WallstepMinimumDuration = 0.25f;
    private const float ForceWallRotationSpeed = 3f;
    private const float WallRunMinimumEntryMomentum = 7f;
    private const float WallRunMinimumMomentum = 7f;
    private const float FlankAlignmentRotationSpeed = 25f;
    private const float FlankWallVacuumStrength = 12f;
    
    // Hard land
    
    private const float HardLandAirDiff = -9;
    private const float HardLandExitMomentum = 4f;
    private const float HardLandRollExitMomentum = 10f;
    private const float HardLandRollMinimumMomentum = 7f;
    private const float HardLandRollForwardSpeed = 14f;

    
    // Hard turn
    
    private const float HardTurnMinimumAngle = 130f;
    private const float HardTurnMinimumMomentum = 8.5f;
    private const float HardTurnMomentumLossModifier = 1.25f;
    
    // Dash

    private const float DashEntryMomentumGain = 5f;
    private const float DashEntryMinimumMomentum = 12f;
    private const float DashsquatTurnMultiplier = 2.25f;
    private const float DashForwardSpeed = 20f;
    
    // --------- End of subclass Fsm data ------------- //


    public override void SetupMachine()
    {
        base.SetupMachine();
        
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
                YVelocity = JumpYVelocity;
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
        
        Machine.Configure(PlayerFsmState.HardLand)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandExitMomentum;
                ReplaceAnimatorTrigger("HardLand");
            });
        
        Machine.Configure(PlayerFsmState.HardLandRoll)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _momentum = HardLandRollExitMomentum;
                ReplaceAnimatorTrigger("HardLandRoll");
            });

        
        Machine.Configure(PlayerFsmState.Jump)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll, _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => _momentum > WallSquatMinimumMomentum && YVelocity < MinYVelocityToInteractWithWall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat, _ => _momentum > WallSquatMinimumMomentum && YVelocity < MinYVelocityToInteractWithWall)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun, _ => _momentum > WallRunMinimumMomentum)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jump");
            });
        
        
        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= CoyoteTime)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff, 1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll, _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Fall");
            });
        
        Machine.Configure(PlayerFsmState.HardTurn)
            .Permit(PlayerFsmTrigger.NoMomentum, PlayerFsmState.GroundMove)
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
                UpdateLedgePosition(FaceLedgeHeight);
                ReplaceAnimatorTrigger("Vault");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.SlowVaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                UpdateLedgePosition(FaceHighLedgeHeight);
                ReplaceAnimatorTrigger("SlowVaultHang");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.MediumVaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                if (!UpdateLedgePosition(FaceHighLedgeHeight)) UpdateLedgePosition(FaceLedgeHeight);
                ReplaceAnimatorTrigger("MediumVaultHang");
                YVelocity = 0;
            });
        
        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
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
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.SlowVaultHang, _ => YVelocity < MediumVaultHangMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.MediumVaultHang, _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                ReplaceAnimatorTrigger("Wallstep");
                YVelocity = Mathf.Lerp(WallstepMinimumYVelocityGain, WallstepMaximumYVelocityGain, ComputeMomentumWeight());
                Animator.SetFloat("VerticalMomentum", ComputeMomentumWeight());
                _momentum = 0;
            });
        
        Machine.Configure(PlayerFsmState.Wallsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            // .Permit(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep, _ => TimeInCurrentState() > WallstepMinimumDuration, 1)
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
        
        Machine.Configure(PlayerFsmState.Wallrun)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(PlayerFsmTrigger.FlankOpen, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                ReplaceAnimatorTrigger("Wallrun");

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
                _momentum = Mathf.Min(Mathf.Max(_momentum + DashEntryMomentumGain, DashEntryMinimumMomentum), MaxMomentum);
                ReplaceAnimatorTrigger("Dash");
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(PlayerFsmState.Jumpsquat, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.Landsquat, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.HardLand, 0.65f);
        StateMapConfig.Duration.Add(PlayerFsmState.HardLandRoll, 0.45f);
        StateMapConfig.Duration.Add(PlayerFsmState.Vault, 0.25f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultHang, 0.975f);
        StateMapConfig.Duration.Add(PlayerFsmState.MediumVaultHang, 0.375f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultFinish, 0.3f);
        StateMapConfig.Duration.Add(PlayerFsmState.Wallsquat, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dashsquat, 0.1f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dash, 0.15f);
        
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallstep, 0.5f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallrun, 0.55f);
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
        if (angle > HardTurnMinimumAngle && _momentum > HardTurnMinimumMomentum)
        {
            Machine.Fire(PlayerFsmTrigger.HardTurn);
        }

        if (_momentum < NoMomentumThreshold)
        {
            Machine.Fire(PlayerFsmTrigger.NoMomentum);
        }

        var forwardRaycastDistance = ComputeDynamicForwardRaycastDistance();
        if (Physics.Raycast(transform.position + Vector3.up * FaceWallHeight, transform.forward,
                forwardRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FaceWall);
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceHighLedgeHeight, transform.forward, 
                       forwardRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(PlayerFsmTrigger.FaceHighLedge);
        } else if (Physics.Raycast(transform.position + Vector3.up * FaceLedgeHeight, transform.forward, 
                       out var hit, forwardRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > 70f) Machine.Fire(PlayerFsmTrigger.FaceLedge);
        }
        else
        {
            Machine.Fire(PlayerFsmTrigger.FaceOpen);
        }
        
        Debug.DrawRay(transform.position + Vector3.up * FaceWallHeight, transform.forward * forwardRaycastDistance, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * FaceHighLedgeHeight, transform.forward * forwardRaycastDistance, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up * FaceLedgeHeight, transform.forward * forwardRaycastDistance, Color.cyan);
        
        var flankRaycastDistance = FlankWallDistance; // I don't think we need to multiply flank distance by RaycastTimeModifier


        var flankRaycastOrigin = transform.position + Vector3.up * FlankWallHeight;
        if (Physics.Raycast(flankRaycastOrigin, transform.right,
                out var hitRight, flankRaycastDistance, ~0, QueryTriggerInteraction.Ignore) &&
            Vector3.Angle(hitRight.normal, -transform.right) < FlankMaximumAngle)
        {
            Machine.Fire(PlayerFsmTrigger.FlankWall);
        } else if (Physics.Raycast(flankRaycastOrigin, -transform.right,
                       out var hitLeft, flankRaycastDistance, ~0, QueryTriggerInteraction.Ignore) &&
                   Vector3.Angle(hitLeft.normal, transform.right) < FlankMaximumAngle)
        {
            Machine.Fire(PlayerFsmTrigger.FlankWall);
        }
        else
        {
            Machine.Fire(PlayerFsmTrigger.FlankOpen);
        }
        
        Debug.DrawRay(flankRaycastOrigin, transform.right * flankRaycastDistance, Color.magenta);
        Debug.DrawRay(flankRaycastOrigin, -transform.right * flankRaycastDistance, Color.magenta);
    }

    private float ComputeDynamicForwardRaycastDistance()
    {
        return Mathf.Lerp(1f, DynamicForwardRaycastMaximumModifier, ComputeMomentumWeight()) * ForwardRaycastDistance * GetRaycastTimeModifier();
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        _inputBuffer.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        OnPlayerPositionUpdated?.Invoke(transform.position, Machine.IsInState(GravityFsmState.Grounded) ||
                                                            Machine.IsInState(PlayerFsmState.ForceWallRotation) ||
                                                            YVelocity < -6f);
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {

            HandleInputMomentumLoss();
            HandleTurning();
            HandleCollisionMove();
            
            SetAnimatorMomentum();
            var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);
        }

        if (Machine.IsInState(PlayerFsmState.SlowVaultHang) || Machine.IsInState(PlayerFsmState.MediumVaultHang) )
        {
            MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
            HandleCollisionMove();
            
        }
        if (Machine.IsInState(PlayerFsmState.SlowVaultFinish))
        {
            HandleTurning(VaultTurningMultiplier, true);
            MoveYOntoLedge(0, SlowVaultFinishLedgeLerpStrength);
            transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
        }
        else if (Machine.IsInState(PlayerFsmState.Vault))
        {
            _momentum = Mathf.Max(_momentum, VaultMinimumMomentum);
            var momentumWeight = ComputeMomentumWeight();
            Animator.SetFloat("SpeedMod", Mathf.Lerp(VaultMinimumAnimatorSpeedMod, VaultMaximumAnimatorSpeedMod, momentumWeight));
            MoveYOntoLedge(0f, VaultLedgeLerpStrength);
            SetAnimatorMomentum();
            transform.position += ComputeDesiredMove();
            HandleTurning(VaultTurningMultiplier, true);
        }
        else if (Machine.IsInState(PlayerFsmState.Wallrun))
        {
            SetAnimatorMomentum();
            UpdateFlankWallNormal();
            HandleFlankAlignment();
            HandleCollisionMove();

            transform.position +=
                ComputeCollisionMove(-_currentFlankWallNormal * (Time.deltaTime * FlankWallVacuumStrength));
        }
        else if (Machine.IsInState(GravityFsmState.Aerial) || Machine.IsInState(PlayerFsmState.Jumpsquat) || Machine.IsInState(PlayerFsmState.Landsquat) || Machine.IsInState(PlayerFsmState.HardTurn))
        {
            Animator.SetFloat("SpeedMod", Mathf.Lerp(0, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()));
            SetAnimatorMomentum();
            HandleCollisionMove();
        }

        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            Animator.SetLayerWeight(1, 0);
        }

        if (Machine.IsInState(PlayerFsmState.HardTurn))
        {
            _momentum = Mathf.Max(0, _momentum - MomentumLossRate * Time.deltaTime * HardTurnMomentumLossModifier);
        }
        
        if (Machine.IsInState(PlayerFsmState.HardLandRoll))
        {
            transform.position += ComputeCollisionMove(transform.forward * (HardLandRollForwardSpeed * Time.deltaTime));
        }
        
        

        if (Machine.IsInState(PlayerFsmState.ForceWallRotation))
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, ForceWallRotationRaycastDistance * GetRaycastTimeModifier(), ~0, QueryTriggerInteraction.Ignore))
            {
                var quaternion = Quaternion.LookRotation(-hit.normal, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, RotationSpeed * Time.deltaTime * ForceWallRotationSpeed);
            }
        }

        if (Machine.IsInState(PlayerFsmState.Dashsquat))
        {
            HandleCollisionMove();
            HandleTurning(DashsquatTurnMultiplier, true);
        }
        if (Machine.IsInState(PlayerFsmState.Dash))
        {
            Animator.SetLayerWeight(1, 0);
            var collisionMove = ComputeCollisionMove(transform.forward * (DashForwardSpeed * Time.deltaTime));
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

    private void MoveYOntoLedge(float yOffset, float lerpStrength)
    {
        var newY = lerpStrength < 0 ? _currentLedgePosition.y : Mathf.Lerp(transform.position.y, _currentLedgePosition.y + yOffset, Time.deltaTime * lerpStrength);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private bool UpdateLedgePosition(float ledgeHeight)
    {
        var downwardRaycastOrigin = transform.position + (Vector3.up * (ledgeHeight + UpdateLedgePositionEpsilon)) + transform.forward * ComputeDynamicForwardRaycastDistance();
        Debug.DrawLine(downwardRaycastOrigin, downwardRaycastOrigin - (Vector3.up * (ledgeHeight + UpdateLedgePositionEpsilon)), Color.green);

        if (!Physics.Raycast(downwardRaycastOrigin, -Vector3.up, out var hit,
                (ledgeHeight + UpdateLedgePositionEpsilon) * GetRaycastTimeModifier(), ~0, QueryTriggerInteraction.Ignore)) return false;
        _currentLedgePosition = hit.point;
        return true;

    }
    
    private void UpdateFlankWallNormal()
    {
        var flankRaycastOrigin = transform.position + Vector3.up * FlankWallHeight;
        if (Physics.Raycast(flankRaycastOrigin, transform.right,
                out var hitRight, FlankWallDistance, ~0, QueryTriggerInteraction.Ignore) &&
            Vector3.Angle(hitRight.normal, -transform.right) < FlankMaximumAngle)
        {
            _currentFlankWallNormal = hitRight.normal;
            _currentFlankSide = false;
        } else if (Physics.Raycast(flankRaycastOrigin, -transform.right,
                       out var hitLeft, FlankWallDistance, ~0, QueryTriggerInteraction.Ignore) &&
                   Vector3.Angle(hitLeft.normal, transform.right) < FlankMaximumAngle)
        {
            _currentFlankWallNormal = hitLeft.normal;
            _currentFlankSide = true;
        }
    }

    private void HandleFlankAlignment()
    {
        var rotationMod = _currentFlankSide ? -1f : 1f;
        var forward = Quaternion.Euler(0f, 90f * rotationMod, 0f) * _currentFlankWallNormal;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * FlankAlignmentRotationSpeed);
    }

    private void HandleTurning(float multiplier = 1f, bool forceForwardInput = false)
    {
        
        var v3 = GetInputMovementVector3();
        var inputVector3 = forceForwardInput ? MirrorInputForward(v3, transform.forward) : v3;
        
        var v2 = GetInputMovementVector2();
        if (v2.magnitude < InputMagnitudeThreshhold)
        {
            inputVector3 = transform.forward;
        }
        
        float momentumWeight = ComputeMomentumWeight();
        var angle = Vector3.SignedAngle(inputVector3.normalized, transform.forward.normalized, Vector3.up);
        var animationDesiredTurnAmount = Mathf.InverseLerp(35f, -35f, angle);
        animationDesiredTurnAmount = Mathf.Lerp(-1, 1, animationDesiredTurnAmount);
        var turnAmount = Animator.GetFloat("TurnAmount");
        var turnLerpSpeed = Mathf.Abs(animationDesiredTurnAmount) > Mathf.Abs(turnAmount) ? 10f : 2f;
        Animator.SetFloat("TurnAmount", Mathf.Lerp(turnAmount, animationDesiredTurnAmount, Time.deltaTime * turnLerpSpeed));
        Animator.SetLayerWeight(1, Mathf.Abs(turnAmount) * momentumWeight);
            
        var momentumDesiredTurnAmount = Mathf.InverseLerp(170f, -170f, angle);
        momentumDesiredTurnAmount = Mathf.Lerp(-1, 1, momentumDesiredTurnAmount);
        _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * Time.deltaTime *
                                              Mathf.Abs(momentumDesiredTurnAmount) * momentumWeight * MomentumTurnLoss));
        
        var quaternion = Quaternion.LookRotation(inputVector3.normalized, Vector3.up);
        
        var lowMomentumRotationMod = _momentum < LowMomentumThreshhold ? LowMomentumRotationMod : 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, RotationSpeed * Time.deltaTime * lowMomentumRotationMod * multiplier);
    }

    private void HandleInputMomentumLoss()
    {
        var v2 = GetInputMovementVector2();
        if (v2.magnitude > InputMagnitudeThreshhold)
        {
            var lowMomentumMomentumGainMod = _momentum < LowMomentumThreshhold ? LowMomentumMomentumGainMod : 1f;
            _momentum = Mathf.Min(MaxMomentum, _momentum + MomentumGainRate  * lowMomentumMomentumGainMod *  Time.deltaTime);
        }
        else
        {
            var lowMomentumMomentumLossMod = _momentum < LowMomentumThreshhold ? LowMomentumMomentumLossMod : 1f;
            _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * lowMomentumMomentumLossMod * Time.deltaTime));
        }
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
        float radius = CollisionMoveSphereCastRadius;
        float castDistance = CollisionMoveSphereCastDistance * GetRaycastTimeModifier();

        Vector3 position = transform.position + Vector3.up * CollisionMoveSphereCastHeight;
        Vector3 direction = output.normalized;

        // SphereCast to account for player volume
        if (Physics.SphereCast(position, radius, direction, out RaycastHit hit, castDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            
            // First collision: slide along the surface
            Vector3 firstNormal = hit.normal;
            output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(firstNormal, Vector3.up));


            // Cast again in the new direction to handle corner (second surface)
            if (Physics.SphereCast(position, radius, output.normalized, out RaycastHit secondHit, output.magnitude))
            {
                Vector3 secondNormal = secondHit.normal;

                // Slide again
                output = Vector3.ProjectOnPlane(output, Vector3.ProjectOnPlane(secondNormal, Vector3.up));
                
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
        var value = Mathf.Lerp(0f, MaximumMomentumSpeedMod, ComputeMomentumWeight());
        return transform.forward.normalized * (MoveSpeed * value * Time.deltaTime);
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
        if (Machine.IsInState(GravityFsmState.Aerial) && YVelocity >MinYVelocityToInteractWithWall - 1f) return;
        
        var collisionRatio = (desiredMove.magnitude + 1f) / (collisionMove.magnitude + 1f);
        _momentum = Mathf.Max(0, _momentum - (MomentumLossRate * Time.deltaTime * (collisionRatio - 1f) * CollisionMomentumLossRate));
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_currentLedgePosition, 0.25f);
    }
}