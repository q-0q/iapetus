public partial class RaceFsm
{
    private void CompleteOnUpdate()
    {
        
    }

    private void CompleteConfigure()
    {
        Machine.Configure(RaceFsmState.Complete)
            .Permit(FsmTrigger.Timeout, RaceFsmState.Inactive)
            .Permit(RaceFsmTrigger.Toggle, RaceFsmState.Disabled)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._active = false;
            });
    }
}