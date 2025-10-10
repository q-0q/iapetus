public partial class RaceFsm
{
    private void ActiveOnUpdate()
    {
        
    }

    private void ActiveConfigure()
    {
        Machine.Configure(RaceFsmState.Active)
            .Permit(RaceFsmTrigger.StartTriggered, RaceFsmState.Complete)
            .Permit(RaceFsmTrigger.Toggle, RaceFsmState.Disabled)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._active = true;
            });
    }
}