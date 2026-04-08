public partial class PlayerFsm
{
    private void KeyItemCollectConfigure()
    {
        Machine.Configure(PlayerFsmState.KeyItemCollect)
            .OnEntry(_ =>
            {
                EndSurge();
                _momentum = 0;
                isSprinting = false;
            })
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle);
    }
}