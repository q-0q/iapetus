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
        public static int WallInteractable;
        public static int Landable;
        public static int AirControl;
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
            GroundMoveOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Jump))
        {
            JumpOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.VaultHang))
        {
            VaultHangOnUpdate();
        }
        if (Machine.IsInState(PlayerFsmState.SlowVaultFinish))
        {
            SlowVaultFinishOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Vault))
        {
            VaultOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Wallrun))
        {
            WallrunOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.LockMomentum))
        {
            LockMomentumOnUpdate();
        }

        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            AerialOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.AirControl))
        {
            AirControlOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.HardTurn))
        {
            HardTurnOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.HardLandRoll))
        {
            HardLandRollOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.ForceWallRotation))
        {
            ForceWallRotationOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.Dashsquat))
        {
            DashsquatOnUpdate();
        }
        if (Machine.IsInState(PlayerFsmState.Grapple))
        {
            GrappleOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.ImpaleGround))
        {
            ImpaleGroundOnUpdate();
        } 
        else if (Machine.IsInState(PlayerFsmState.ImpaleAir))
        {
            ImpaleAirOnUpdate();
        } else
        {
            Animator.SetLayerWeight(2, Mathf.Lerp(Animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
        }

        if (Machine.IsInState(PlayerFsmState.GrappleStartup))
        {
            GrappleStartupOnUpdate();
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