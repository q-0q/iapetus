public partial class CrumbleFsm
{

    private void FormingConfigure()
    {
        Machine.Configure(CrumbleFsmState.Forming)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Idle);
    }
}