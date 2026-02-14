using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Code.PlayerComponents;
using DG.Tweening;
using FMOD.Studio;
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
        public static int WalkToPosition;
        public static int WalkToSwitchPosition;
        public static int InteractWithSwitch;
        public static int Dash;
        public static int FallAfterDash;
        public static int Skipsquat;
        public static int Skip;
        public static int LandsquatAfterDash;
        public static int DashVault;
        public static int Dialogue;
        public static int WalkToDialoguePosition;
        public static int Interactable;
        public static int Climb;
        public static int PitonHoming;
        public static int Pitonsquat;
        public static int PitonFlipsquat;
        public static int PitonFlip;
        public static int PitonInteractable;
        public static int FallAfterPitonHoming;
        
        public static int Slide;
        public static int TightropeMove;
        public static int GroundMoveAfterVault;

        public static int CutsceneWary;
        public static int CutsceneIdle;
        public static int Updraft;

        public static int TrialTeleport;
        public static int Dying1;
        public static int Dying2;
        public static int Dead;
        public static int FallAfterSlide;
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
        public static int FaceOpenLenient;
        public static int FlankWall;
        public static int FlankOpen;
        public static int Dash;
        public static int Attack;
        public static int ContactHitboxTrigger;
        public static int InteractWithSwitch;
        public static int ArriveAtWalkToPositionTarget;
        public static int ArriveAtWalkToPositionTargetRanged;
        public static int StartDialogue;
        public static int EndDialogue;
        
        public static int StartUpdraft;
        public static int EndUpdraft;

        public static int ArriveAtPiton;
        public static int EnterPitonTrigger;
    }
    
    protected override void OnAwake()
    {
        Singleton = this;
        
        var saveData = SaveSystem.LoadSaveData(0);
        if (saveData != null )
        {
            if (saveData.playerInGamePosition != null)
            {
                if (saveData.playerInGamePosition.Length != 0)
                {
                    transform.position = new Vector3(saveData.playerInGamePosition[0], saveData.playerInGamePosition[1],
                        saveData.playerInGamePosition[2]);

                    transform.rotation = Quaternion.Euler(0, saveData.playerInGameYAngle, 0);
                }
            }
        }

        ApplyMetaSaveData(MetaSaveSystem.LoadMetaSaveData());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 240;
    }
    
    protected override void OnStart()
    {
        
        base.OnStart();
        TryGetComponent(out Animator);
        Singleton = this;
        InitState = PlayerFsmState.GroundMove;
        isSprinting = false;
        Time.timeScale = 1f;
        _movementAnimationMirror = false;
        TryGetComponent(out _playerInput);
        _inputBuffer = new InputBuffer(_playerInput, 0.275f);
        _inputBuffer.InitInput("Jump");
        _inputBuffer.InitInput("Dash");
        _inputBuffer.InitInput("Attack");
        _inputBuffer.InitInput("Interact");
        _camera = Camera.main;
        _previousWallrunSide = FlankType.None;
        _checkpointVector3 = transform.position;
        _checkpointQuaternion = transform.rotation;
        _kiIndicatorParticles = transform.Find("Armature").GetComponentsInChildren<ParticleSystem>().Where(d => d.name == "PlayerFootParticles").ToList();
        _teleportParticles = transform.Find("TeleportParticles").GetComponent<ParticleSystem>();
        _deathParticles = transform.Find("DeathParticles").GetComponent<ParticleSystem>();
        _teleportParticles.transform.SetParent(null);
        _deathParticles.transform.SetParent(null);
        
        // transform.Find("KiIndicatorParticles").SetParent(null);
        _renderers = GetComponentsInChildren<Renderer>().ToList();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        Shader.SetGlobalFloat("_PlayerTintWeight", 0);
        
        SnapToGround();
    }


    public override void OnUpdate()
    {
        if (HitstopOnUpdate()) return;

        var isCutscenePlayerDisabled = CutsceneManager.Singleton.IsCutscenePlayerDisabled();
        if (!isCutscenePlayerDisabled)
        {
            _inputBuffer.OnUpdate();
        }
        
        OnPlayerMomentumUpdated?.Invoke(_momentum);
        OnPlayerPositionUpdated?.Invoke(transform.position, Machine.IsInState(GravityFsmState.Grounded) ||
                                                            Machine.IsInState(PlayerFsmState.ForceWallRotation) ||
                                                            YVelocity < -6f);
        _timeSinceDashFinished += Time.deltaTime;
        _comboTimer += Time.deltaTime;
        if (_comboTimer > ComboTimeoutDuration)
        {
            ResetCombo();
        }
        
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerMomentum", ComputeMomentumWeight());
        UpdateShaderGlobals();
        HandleSlopeTimer();



        if (Machine.IsInState(PlayerFsmState.GroundMove))
        {
            GroundMoveOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.TightropeMove))
        {
            TightropeMoveOnUpdate();
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
        
        if (Machine.IsInState(PlayerFsmState.Wallsquat))
        {
            WallsquatOnUpdate();
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
        
        if (Machine.IsInState(PlayerFsmState.HardLand))
        {
            HardLandOnUpdate();
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
        if (Machine.IsInState(PlayerFsmState.Dash))
        {
            DashOnUpdate();
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
        
        if (Machine.IsInState(PlayerFsmState.WalkToPosition))
        {
            WalkToPositionOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.InteractWithSwitch))
        {
            InteractWithSwitchOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.FallAfterDash))
        {
            FallAfterDashOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Skipsquat))
        {
            SkipsquatOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Skip))
        {
            SkipOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.Interactable) && !isCutscenePlayerDisabled)
        {
            InteractableOnUpdate();
        }
        else
        {
            currentPotentialInteractable = null;
        }
        
        if (Machine.IsInState(PlayerFsmState.Dialogue))
        {
            DialogueOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Slide))
        {
            SlideOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Updraft))
        {
            UpdraftOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.TrialTeleport))
        {
            TrialTeleportOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.Climb))
        {
            ClimbOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.PitonHoming))
        {
            PitonHomingOnUpdate();
        }

        if (Machine.IsInState(PlayerFsmState.Pitonsquat))
        {
            PitonsquatOnUpdate();
        }
        
        if (Machine.IsInState(PlayerFsmState.Dying1))
        {
            DyingOnUpdate();
        }

        
        if (_playerInput.actions["Reset"].WasPerformedThisFrame())
        {
            InvokePlayerDeath();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            SaveSystem.WritePlayerInGamePosition(transform.position, 0, 0);
        }
        
        if (transform.position.y < -80f)
        {
            InvokePlayerDeath();
        }
        

        // HandleSlopeTimer();
         HandleKiEffects();
        
        base.OnUpdate();
        
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
    
    private void OnStateChangedCompleted(TriggerParams obj)
    {
        // print(InheritableEnum.GetFieldNameByValue(Machine.State(), typeof(PlayerFsmState)));
        ReplaceAnimatorTrigger(StateMapConfig.AnimationTrigger.GetStrict(this));
    }

    private void OnEnable()
    {
        PlayerContactCollider.OnPlayerContactHitboxCollision += OnContactHitboxCollide;
        MetaSaveSystem.OnMetaSaveDataUpdated += ApplyMetaSaveData;
        PlayerFootTracker.OnPlayerFootstep += OnPlayerFootstep;
        
        activeFmodInstance = FMODUnity.RuntimeManager.CreateInstance(comboActiveFmodEvent);
    }
    
    private void OnDisable()
    {
        PlayerContactCollider.OnPlayerContactHitboxCollision -= OnContactHitboxCollide;
        MetaSaveSystem.OnMetaSaveDataUpdated -= ApplyMetaSaveData;
        PlayerFootTracker.OnPlayerFootstep -= OnPlayerFootstep;
        activeFmodInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
}