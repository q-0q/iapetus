using UnityEngine;

public partial class PlayerFsm
{
    public override void SetupMachine()
    {
        print("player setupmachine");
        base.SetupMachine();

        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ => { ReplaceAnimatorTrigger("GroundMove"); });

        Machine.Configure(PlayerFsmState.Jumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Jumpsquat");
                _inputBuffer.ConsumeBuffer("Jump");
            })
            .OnExitFrom(FsmTrigger.Timeout, _ => { YVelocity = JumpYVelocity; });

        Machine.Configure(PlayerFsmState.Landsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Landsquat"); })
            .OnExit(_ =>
            {
                _movementAnimationMirror = !_movementAnimationMirror;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
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
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleAir, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            // _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Jump"); });


        Machine.Configure(PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat, _ => TimeInAir <= CoyoteTime)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .Permit(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat)
            .OnEntry(_ => { ReplaceAnimatorTrigger("Fall"); });

        Machine.Configure(PlayerFsmState.HardTurn)
            .Permit(PlayerFsmTrigger.NoMomentum, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ => { ReplaceAnimatorTrigger("HardTurn"); });

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

        Machine.Configure(PlayerFsmState.VaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                if (!UpdateLedgePosition(FaceHighLedgeHeight)) UpdateLedgePosition(FaceLedgeHeight);
                YVelocity = 0;
            });

        Machine.Configure(PlayerFsmState.SlowVaultHang)
            .SubstateOf(PlayerFsmState.VaultHang)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("SlowVaultHang");
            });

        Machine.Configure(PlayerFsmState.MediumVaultHang)
            .SubstateOf(PlayerFsmState.VaultHang)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("MediumVaultHang");
            });

        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            // .SubstateOf(PlayerFsmState.Grounded)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("SlowVaultFinish");
                YVelocity = 0;
            })
            .OnExit(_ => { _momentum = 3f; });

        Machine.Configure(PlayerFsmState.Wallstep)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(GravityFsmTrigger.StartFrameWithNegativeYVelocity, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.SlowVaultHang,
                _ => YVelocity < MediumVaultHangMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang,
                _ => YVelocity > MediumVaultHangMinimumYVelocity, 1)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                ReplaceAnimatorTrigger("Wallstep");
                YVelocity = Mathf.Lerp(WallstepMinimumYVelocityGain, WallstepMaximumYVelocityGain,
                    ComputeMomentumWeight());
                Animator.SetFloat("VerticalMomentum", ComputeMomentumWeight());
                _momentum = 0;
            });

        Machine.Configure(PlayerFsmState.Wallsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            // .Permit(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep,
                _ => TimeInCurrentState() > WallstepMinimumDuration, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                ReplaceAnimatorTrigger("Wallsquat");
            })
            .OnExitFrom(PlayerFsmTrigger.FaceOpen, _ => { _momentum = 0; });

        Machine.Configure(PlayerFsmState.Wallrun)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.FlankOpen, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .OnEntry(_ =>
            {
                print("Yvelocity on entering wallrun: " + YVelocity);
                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                ReplaceAnimatorTrigger("Wallrun");
            })
            .OnExitFrom(PlayerFsmTrigger.Jump, _ =>
            {
                var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
                var forward = Quaternion.Euler(0f, WallrunJumpAngle * rotationMod, 0f) * _currentFlankWallNormal;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            })
            .OnExit(_ => { _previousWallrunSide = _currentFlankType; });

        Machine.Configure(PlayerFsmState.Dashsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Grapple)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Dash");
                ReplaceAnimatorTrigger("Dashsquat");
            });

        Machine.Configure(PlayerFsmState.Grapple)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GrappleFlipsquat)
            // .Permit(PlayerFsmTrigger.ContactHitboxTrigger, PlayerFsmState.GrappleFlipsquat)
            // .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat, _ => true)
            // .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat, _ => true)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                _momentum = Mathf.Min(Mathf.Max(_momentum + DashEntryMomentumGain, DashEntryMinimumMomentum),
                    MaxMomentum);
                transform.rotation =
                    Quaternion.LookRotation(PlayerWeaponFsm.Singleton.transform.position - transform.position,
                        Vector3.up);
                ReplaceAnimatorTrigger("Dash");
            })
            .OnExit(_ =>
            {
                transform.position = PlayerWeaponFsm.Singleton.transform.position - transform.forward * 0.75f;
            });


        Machine.Configure(PlayerFsmState.ImpaleGround)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Attack");
                OnPlayerImpaleStateEntered?.Invoke();
                Animator.SetTrigger("Impale");
                _stateEntryMomentum = _momentum;
            }).OnExit(_ => { Animator.ResetTrigger("Impale"); });
        ;

        Machine.Configure(PlayerFsmState.ImpaleAir)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Attack");
                OnPlayerImpaleStateEntered?.Invoke();
                Animator.SetTrigger("ImpaleJump");
                _stateEntryMomentum = _momentum;
            }).OnExit(_ => { Animator.ResetTrigger("ImpaleJump"); });
        ;

        Machine.Configure(PlayerFsmState.GrappleStartup)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Grapple)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("GrappleStartup");
                YVelocity = 10;
                _inputBuffer.ConsumeBuffer("Attack");
            }).OnExit(_ => { YVelocity = 0; });

        Machine.Configure(PlayerFsmState.GrappleFlipsquat)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GrappleFlip)
            .OnEntry(_ =>
            {
                // transform.DOShakePosition(0.5f, 0.3f);
                ReplaceAnimatorTrigger("GrappleFlipsquat");
                // HitstopManager.Singleton.StartHitstop(0.075f);
            })
            .OnExit(_ => { });

        Machine.Configure(PlayerFsmState.GrappleFlip)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLand, _ => AirYDiff() < HardLandAirDiff,
                1)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.HardLandRoll,
                _ => AirYDiff() < HardLandAirDiff && _momentum > HardLandRollMinimumMomentum, 2)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWall, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum && YVelocity < WallsquatMinimumYVelocity)
            .PermitIf(PlayerFsmTrigger.FlankWall, PlayerFsmState.Wallrun,
                _ => _momentum > WallRunMinimumMomentum && YVelocity < WallRunMinimumYVelocity)
            .OnEntry(_ =>
            {
                _momentum = 10f;
                ReplaceAnimatorTrigger("GrappleFlip");
                YVelocity = 30;
            });
    }

    public override void SetupStateMaps()
    {
        print("player setupstatemaps");

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
        StateMapConfig.Duration.Add(PlayerFsmState.Grapple, 0.1f);
        StateMapConfig.Duration.Add(PlayerFsmState.ImpaleGround, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.ImpaleAir, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.GrappleStartup, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.GrappleFlipsquat, 0.265f);

        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallstep, 0.5f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallrun, 0.55f);
    }
}