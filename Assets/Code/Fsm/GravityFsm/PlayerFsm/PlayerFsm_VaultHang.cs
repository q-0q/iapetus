public partial class PlayerFsm
{
    private void VaultHangOnUpdate()
    {
        UpdateLedgePosition(FaceHighLedgeHeight);
        MoveYOntoLedge(VaultHangLedgeYOffset, VaultHangLedgeLerpStrength);
        HandleCollisionMove();
    }
}