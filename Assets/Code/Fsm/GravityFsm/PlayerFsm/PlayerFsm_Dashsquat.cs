public partial class PlayerFsm
{
    private void DashsquatOnUpdate()
    {
        HandleCollisionMove();
        HandleTurning(DashsquatTurnMultiplier, true);
    }
}