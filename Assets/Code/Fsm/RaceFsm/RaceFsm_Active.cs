public partial class RaceFsm
{
    private void ActiveOnUpdate()
    {
        
    }

    private void ActiveConfigure()
    {
        Machine.Configure(RaceFsmState.Active)
            .Permit(RaceFsmTrigger.StartTriggered, RaceFsmState.Complete)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._active = true;
            });
    }
}