public partial class RaceFsm
{
    private void StartOnUpdate()
    {

    }

    private void StartConfigure()
    {
        Machine.Configure(RaceFsmState.Start)
            .Permit(RaceFsmTrigger.StartNotTriggered, RaceFsmState.Active)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._timer = 0;
                UiTimer.Singleton._display = true;
            });
    }
}