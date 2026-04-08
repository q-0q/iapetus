public partial class PlayerFsm
{
    private void KeyItemCollectConfigure()
    {
        Machine.Configure(PlayerFsmState.KeyItemCollect)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle);
    }
}