public partial class PlayerFsm
{
    private void VaultHangOnUpdate()
    {
        UpdateLedgePosition(FaceHighLedgeHeight);
        MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
        HandleCollisionMove();
    }

    private void VaultHangConfigure()
    {
        Machine.Configure(PlayerFsmState.VaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                if (!UpdateLedgePosition(FaceHighLedgeHeight)) UpdateLedgePosition(FaceLedgeHeight);
                YVelocity = 0;
            });
    }
}