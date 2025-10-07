using UnityEngine;

public partial class PlayerFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();

        GroundMoveConfigure();
        JumpsquatConfigure();
        LandsquatConfigure();
        HardLandConfigure();
        HardLandRollConfigure();
        HardTurnConfigure();
        JumpConfigure();
        FallConfigure();
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
        ImpaleGroundConfigure();
        ImpaleAirConfigure();
        GrappleStartupConfigure();
        GrappleFlipConfigure();
        GrappleFlipsquatConfigure();
        WallInteractableConfigure();
        LandableConfigure();
        
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
        
        StateMapConfig.IsAbstract.Add(PlayerFsmState.Landable, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.ForceWallRotation, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.LockMomentum, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.VaultHang, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.WallInteractable, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.AirControl, true);
        StateMapConfig.IsAbstract.Add(PlayerFsmState.IgnoreFailsafe, true);

        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallstep, 0.5f);
        StateMapConfig.GravityStrengthMod.Add(PlayerFsmState.Wallrun, 0.55f);
    }
}