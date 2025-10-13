public partial class PlayerFsm
{
    private void VaultHangOnUpdate()
    {
        UpdateLedgePosition(FaceHighLedgeHeight);
        MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
        DoGenericCollisionMove();
        Animator.SetLayerWeight(1, 0);
    }

    private void VaultHangConfigure()
    {
        Machine.Configure(PlayerFsmState.VaultHang)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .SubstateOf(GravityFsmState.LockTightropeColliderPosition)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SlowVaultFinish)
            .OnEntry(_ =>
            {
                if (!UpdateLedgePosition(FaceHighLedgeHeight)) UpdateLedgePosition(FaceLedgeHeight);
                YVelocity = 0;
            });
    }
}