using UnityEngine;

public partial class PlayerFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();
        
        Machine.OnTransitionCompleted(OnStateChangedCompleted);

        GroundMoveConfigure();
        JumpsquatConfigure();
        LandsquatConfigure();
        HardLandConfigure();
        HardLandRollConfigure();
        HardTurnConfigure();
        JumpConfigure();
        FallConfigure();
        FallAfterDashConfigure();
        AerialConfigure();
        VaultConfigure();
        VaultHangConfigure();
        SlowVaultHangConfigure();
        MediumVaultHangConfigure();
        SlowVaultFinishConfigure();
        WallstepConfigure();
        WallsquatConfigure();
        WallrunConfigure();
        DashsquatConfigure();
        GrappleConfigure();
        DashConfigure();
        ImpaleGroundConfigure();
        ImpaleAirConfigure();
        GrappleStartupConfigure();
        GrappleFlipConfigure();
        GrappleFlipsquatConfigure();
        WallInteractableConfigure();
        LandableConfigure();
        WalkToPositionConfigure();
        WalkToSwitchPositionConfigure();
        InteractWithSwitchConfigure();
        SkipsquatConfigure();
        SkipConfigure();
        InteractableConfigure();
        WalkToDialoguePositionConfigure();
        DialogueConfigure();
        SlideConfigure();
        TightropeMoveConfigure();
        CutsceneConfigure();
        UpdraftConfigure();
        TrialTeleportConfigure();
        ClimbConfigure();
        PitonConfigure();
        DeathConfigure();
        StepConfigure();
        SwimConfigure();
        RopeSwingConfigure();
    }

    public override void SetupStateMaps()
    {
        

        base.SetupStateMaps();
        StateMapConfig.Duration.Add(PlayerFsmState.Jumpsquat, 0.145f);
        StateMapConfig.Duration.Add(PlayerFsmState.Landsquat, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.HardLand, 0.65f);
        StateMapConfig.Duration.Add(PlayerFsmState.HardLandRoll, 0.35f);
        StateMapConfig.Duration.Add(PlayerFsmState.Vault, 0.25f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultHang, 0.975f);
        StateMapConfig.Duration.Add(PlayerFsmState.MediumVaultHang, 0.375f);
        StateMapConfig.Duration.Add(PlayerFsmState.SlowVaultFinish, 0.3f);
        StateMapConfig.Duration.Add(PlayerFsmState.Wallsquat, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dashsquat, 0.13f);
        StateMapConfig.Duration.Add(PlayerFsmState.Grapple, 0.1f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dash, 0.25f);
        StateMapConfig.Duration.Add(PlayerFsmState.ImpaleGround, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.ImpaleAir, 0.55f);
        StateMapConfig.Duration.Add(PlayerFsmState.GrappleStartup, 0.175f);
        StateMapConfig.Duration.Add(PlayerFsmState.GrappleFlipsquat, 0.265f);
        StateMapConfig.Duration.Add(PlayerFsmState.InteractWithSwitch, 0.65f);
        StateMapConfig.Duration.Add(PlayerFsmState.Skipsquat, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.TrialTeleport, TrialTeleportDuration);
        StateMapConfig.Duration.Add(PlayerFsmState.PitonFlipsquat, 0.1f);
        StateMapConfig.Duration.Add(PlayerFsmState.PitonFlip, 0.9f);
        StateMapConfig.Duration.Add(PlayerFsmState.PitonHoming, 1f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dying1, 0.3f);
        StateMapConfig.Duration.Add(PlayerFsmState.Dying2, 0.5f);
        StateMapConfig.Duration.Add(PlayerFsmState.FallAfterSlideLateral, 0.25f);
        StateMapConfig.Duration.Add(PlayerFsmState.StepStart, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.StepEnd, 0.125f);
        StateMapConfig.Duration.Add(PlayerFsmState.RopeSwingHoming, 0.15f);
        StateMapConfig.Duration.Add(PlayerFsmState.RopeSwingJumpsquat, 0.15f);
        
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Dash, "Dash");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Dashsquat, "Dashsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Fall, "Fall");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.FallAfterDash, "FallAfterDash");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.GroundMove, "GroundMove");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.HardLand, "HardLand");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.HardLandRoll, "HardLandRoll");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.HardTurn, "HardTurn");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.InteractWithSwitch, "InteractWithSwitch");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Jump, "Jump");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Jumpsquat, "Jumpsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Landsquat, "Landsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.MediumVaultHang, "MediumVaultHang");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SlowVaultFinish, "SlowVaultFinish");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SlowVaultHang, "SlowVaultHang");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Vault, "Vault");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.WalkToPosition, "GroundMove");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Wallsquat, "Wallsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Wallstep, "Wallstep");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Skipsquat, "Skipsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Skip, "Skip");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.LandsquatAfterDash, "LandsquatAfterDash");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.DashVault, "DashVault");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SlideLateral, "SlideLateral");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SlideDown, "SlideDown");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.FallAfterSlideLateral, "FallAfterSlideLateral");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.GroundMoveAfterVault, "GroundMoveAfterVault");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.CutsceneWary, "Wary");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.CutsceneIdle, "GroundMove");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Updraft, "Updraft");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.TrialTeleport, "GroundMove");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Climb, "Climb");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.PitonFlipsquat, "PitonFlipsquat");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.PitonFlip, "PitonFlip");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.PitonHoming, "PitonHoming");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.FallAfterPitonHoming, "Fall");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Dying2, "Dying");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.FallAfterSlide, "Fall");
        
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Idle, "Idle");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.StepStart, "StepStart");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.StepEnd, "StepEnd");
        
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SwimSurfaceRise, "SwimSurfaceRise");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.SwimSurface, "SwimSurface");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.DiveFall, "DiveFall");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.Drown, "Drown");
        
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.RopeSwingHoming, "RopeSwing");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.RopeSwing, "RopeSwing");
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.RopeSwingJumpsquat, "RopeSwingJumpsquat", 2);
        StateMapConfig.AnimationTrigger.Add(PlayerFsmState.RopeSwingJump, "RopeSwingJump");

        StateMapConfig.IsAbstract.Add(PlayerFsmState.Landable, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.ForceWallRotation, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.LockMomentum, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.VaultHang, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.WallInteractable, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.AirControl, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.WalkToPosition, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.Slide, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.Swim, true);

        // StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Dashsquat, 0.5f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallstep, 0.5f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallrun, 0.45f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.SlideLateral, 0.35f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.SlideDown, 0.35f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Skip, 0.8f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.PitonFlip, 0.75f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Swim, 0.75f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.RopeSwingJump, 0.95f);
        
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Landsquat, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Jumpsquat, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Skipsquat, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.TightropeMove, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Wallsquat, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Wallstep, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.MediumVaultHang, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.SlowVaultHang, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.SlowVaultFinish, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.Vault, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.HardLand, true);
        StateMapConfig.LockSpringCollider.Add(PlayerFsmState.HardLandRoll, true);
        
        StateMapConfig.TightropeLineOffset.Add(PlayerFsmState.VaultHang, new Vector3(0, 2.5f, 1.0f));
        StateMapConfig.TightropeLineOffset.Add(PlayerFsmState.Wallsquat, new Vector3(0, 2.5f, 1.0f));
        StateMapConfig.TightropeLineYLerpStrength.Add(PlayerFsmState.SlowVaultFinish, 50f);
        StateMapConfig.TightropeLineYLerpStrength.Add(PlayerFsmState.Wallstep, 5f);
    }
}