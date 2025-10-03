using DG.Tweening;
using UnityEngine;

public partial class PlayerWeaponFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();

        IdleConfigure();
        ImpaleStartupConfigure();
        ImpaleActiveConfigure();
        ImpaleRecoveryConfigure();
        ImpaleStuckConfigure();
        ImpaleStuckRecoveryConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStartup, 0.35f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleActive, 0.25f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleRecovery, 0.25f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStuck, 0.95f);
        StateMapConfig.Duration.Add(PlayerWeaponFsmState.ImpaleStuckRecovery, 0.1f);
    }
}