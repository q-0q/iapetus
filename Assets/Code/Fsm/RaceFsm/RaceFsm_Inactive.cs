public partial class RaceFsm
{
    private void InactiveOnUpdate()
    {

    }

    private void InactiveConfigure()
    {
        Machine.Configure(RaceFsmState.Inactive)
            .Permit(RaceFsmTrigger.StartTriggered, RaceFsmState.Start)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._display = false;
                UiTimer.Singleton._active = false;
            });
    }
}