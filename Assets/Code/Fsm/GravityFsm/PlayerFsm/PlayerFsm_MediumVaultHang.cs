public partial class PlayerFsm
{
    private void MediumVaultHangConfigure()
    {
        Machine.Configure(PlayerFsmState.MediumVaultHang)
            .SubstateOf(PlayerFsmState.VaultHang);
    }
}