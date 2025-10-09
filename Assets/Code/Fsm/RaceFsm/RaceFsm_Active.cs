public partial class RaceFsm
{
    private void ActiveOnUpdate()
    {
        
    }

    private void ActiveConfigure()
    {
        Machine.Configure(RaceFsmState.Active)
            .OnEntry(_ =>
            {
                UiTimer.Singleton._active = true;
            });
    }
}