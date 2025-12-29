public partial class PlayerFsm
{
    private void SlowVaultHangConfigure()
    {
        Machine.Configure(PlayerFsmState.SlowVaultHang)
            .SubstateOf(PlayerFsmState.VaultHang)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(climbFmodEvent, gameObject);
            });
    }
}