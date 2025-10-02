using System;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Wasp;

public partial class PlayerFsm : GravityFsm
{
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
        public static int Grapple;
        public static int HardLand;
        public static int HardLandRoll;
        public static int Wallrun;
        public static int ImpaleGround;
        public static int ImpaleAir;
        public static int GrappleStartup;
        public static int GrappleFlipsquat;
        public static int GrappleFlip;
        public static int VaultHang;
        public static int LockMomentum;
    }

    public class PlayerFsmTrigger : GravityFsmTrigger
    {
        public static int Jump;
        public static int HardTurn;
        public static int NoMomentum;
        public static int FaceLedge;
        public static int FaceHighLedge;
        public static int FaceWall;
        public static int FaceWallStrict;
        public static int FaceOpen;
        public static int FlankWall;
        public static int FlankOpen;
        public static int Dash;
        public static int Attack;
        public static int ContactHitboxTrigger;
    }
    
    protected override void OnAwake()
    {
        Singleton = this;
    }
    
    protected override void OnStart()
    {
        print("player onstart");
        base.OnStart();
        Singleton = this;
        InitState = PlayerFsmState.GroundMove;
        print("init state: " + InitState);
        _movementAnimationMirror = false;
        TryGetComponent(out _playerInput);
        _inputBuffer = new InputBuffer(_playerInput, 0.275f);
        _inputBuffer.InitInput("Jump");
        _inputBuffer.InitInput("Dash");
        _inputBuffer.InitInput("Attack");
        _camera = Camera.main;
        _previousWallrunSide = FlankType.None;
        
        // QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        // Application.targetFrameRate = 30;
        
    }
    

    public override void OnUpdate()
    {
        if (HitstopOnUpdate()) return;
        
        base.OnUpdate();
        _inputBuffer.OnUpdate();
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        OnPlayerPositionUpdated?.Invoke(transform.position, Machine.IsInState(GravityFsmState.Grounded) ||
                                                            Machine.IsInState(PlayerFsmState.ForceWallRotation) ||
                                                            YVelocity < -6f);
        
        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {
            
            HandleInputMomentumChange();
            HandleTurning();
            HandleCollisionMove();
            
            SetAnimatorMomentum();
            var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);
        }

        if (Machine.IsInState(PlayerFsmState.VaultHang))
        {
            UpdateLedgePosition(FaceHighLedgeHeight);
            MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
            HandleCollisionMove();
            
        }
        if (Machine.IsInState(PlayerFsmState.SlowVaultFinish))
        {
            HandleTurning(VaultTurningMultiplier, true);
            MoveYOntoLedge(0f, SlowVaultFinishLedgeLerpStrength);
            transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
        }
        
        if (Machine.IsInState(PlayerFsmState.Vault))
        {
            _momentum = Mathf.Max(_momentum, VaultMinimumMomentum);
            var momentumWeight = ComputeMomentumWeight();
            Animator.SetFloat("SpeedMod", Mathf.Lerp(VaultMinimumAnimatorSpeedMod, VaultMaximumAnimatorSpeedMod, momentumWeight));
            MoveYOntoLedge(0f, VaultLedgeLerpStrength);
            SetAnimatorMomentum();
            transform.position += ComputeCollisionMove(ComputeDesiredMove());
            HandleTurning(VaultTurningMultiplier, true);
        }
        
        if (Machine.IsInState(PlayerFsmState.Wallrun))
        {
            SetAnimatorMomentum();
            HandleFlankAlignment();
            HandleCollisionMove();

            transform.position +=
                ComputeCollisionMove(-_currentFlankWallNormal * (Time.deltaTime * FlankWallVacuumStrength));
        }
        
        if (Machine.IsInState(PlayerFsmState.LockMomentum))
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
            Animator.SetLayerWeight(2, 0);
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
        if (Machine.IsInState(PlayerFsmState.Grapple))
        {
            Animator.SetLayerWeight(1, 0);
            var collisionMove = ComputeCollisionMove(transform.forward * (DashForwardSpeed * Time.deltaTime));
            transform.position += collisionMove;
        }
        
        if (Machine.IsInState(PlayerFsmState.ImpaleGround))
        {
            Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 1, Time.deltaTime * 90f));
            Animator.SetLayerWeight(1, 0);
            HandleInputMomentumChange();

            HandleTurning(0.75f, true);
            HandleCollisionMove(ImpaleMovementModifier);
            
            SetAnimatorMomentum();
            var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);

            var targetMomentum = _stateEntryMomentum < ImpaleMinimumMomentumAfterOffset ? _momentum : Mathf.Max(_stateEntryMomentum + ImpaleMomentumOffset, ImpaleMinimumMomentumAfterOffset);
            _momentum = Mathf.Lerp(_momentum, targetMomentum, Time.deltaTime * ImpaleMomentumLerpStrenth);
        } 
        else if (Machine.IsInState(PlayerFsmState.ImpaleAir))
        {
            Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 1, Time.deltaTime * 90f));
            Animator.SetLayerWeight(1, 0);
            
            var speedMod = Mathf.Lerp(0f, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight());
            Animator.SetFloat("SpeedMod", speedMod);

            var targetMomentum = _stateEntryMomentum < ImpaleMinimumMomentumAfterOffset ? _momentum : Mathf.Max(_stateEntryMomentum + ImpaleMomentumOffset, ImpaleMinimumMomentumAfterOffset);
            _momentum = Mathf.Lerp(_momentum, targetMomentum, Time.deltaTime * ImpaleMomentumLerpStrenth);
        } else
        {
            Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
        }

        if (Machine.IsInState(PlayerFsmState.GrappleStartup))
        {
            Animator.SetLayerWeight(2, 0);
            var transformPosition = new Vector3(PlayerWeaponFsm.Singleton.transform.position.x, transform.position.y, PlayerWeaponFsm.Singleton.transform.position.z);
            var forward = transformPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * GrappleStartupRotationLerpStrength);

            var destinationPosition = new Vector3(transform.position.x,
                PlayerWeaponFsm.Singleton.transform.position.y + GrappleStartupYPositionOffset, transform.position.z);
            
            transform.position = Vector3.Lerp(transform.position,
                destinationPosition,
                Time.deltaTime * GrappleStartupYPositionLerpStrength);
            
            _momentum = Mathf.Max(0, _momentum - MomentumLossRate * Time.deltaTime * GrappleStartupMomentumLossMod);
            HandleCollisionMove();
            
        }


        if (_playerInput.actions["Reset"].WasPerformedThisFrame())
        {
            transform.position = _checkpointVector3;
            transform.rotation = _checkpointQuaternion;
            _momentum = 0;
            YVelocity = 0;
        }
    }

    private bool HitstopOnUpdate()
    {
        if (HitstopManager.Singleton.IsHitstopActive())
        {
            Animator.enabled = false;
            return true;
        }
        else
        {
            Animator.enabled = true;
        }

        return false;
    }

    private void OnEnable()
    {
        PlayerContactCollider.OnPlayerContactHitboxCollision += OnContactHitboxCollide;
    }
    
    private void OnDisable()
    {
        PlayerContactCollider.OnPlayerContactHitboxCollision -= OnContactHitboxCollide;
    }
}