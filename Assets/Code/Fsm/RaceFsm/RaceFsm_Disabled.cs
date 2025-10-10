public partial class RaceFsm
{
    private void DisabledOnUpdate()
    {

    }

    private void DisabledConfigure()
    {
        Machine.Configure(RaceFsmState.Disabled)
            .Permit(RaceFsmTrigger.Toggle, RaceFsmState.Inactive)
            .OnEntry(_ => { DisabledOnEnter(); });
    }

    private void DisabledOnEnter()
    {
        _currentTriggerId = -1;
        UiTimer.Singleton._display = false;
        UiTimer.Singleton._active = false;
        
        foreach (var t in Triggers)
        {
            t.Hide();
        }
    }
}