public partial class PlayerFsm
{
    private void MajorLeylineNodeInteractConfigure()
    {
        Machine.Configure(PlayerFsmState.MajorLeylineNodeInteract)
            .OnEntry(_ =>
            {
                EndSurge();
                _momentum = 0;
                isSprinting = false;
            })
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle);
    }
}