public partial class PlayerFsm
{
    private void VaultHangOnUpdate()
    {
        UpdateLedgePosition(FaceHighLedgeHeight);
        MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
        HandleCollisionMove();
        Animator.SetLayerWeight(1, 0);
    }

    private void VaultHangConfigure()
    {
        Machine.Configure(PlayerFsmState.VaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                if (!UpdateLedgePosition(FaceHighLedgeHeight + GetCurrentDashRaycastHeightOffset())) UpdateLedgePosition(FaceLedgeHeight);
            })
            .OnExit(_ =>
            {
                YVelocity = 0;
            });
    }
}