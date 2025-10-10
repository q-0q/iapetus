public partial class RaceFsm
{
    private void InactiveOnUpdate()
    {

    }

    private void InactiveConfigure()
    {
        Machine.Configure(RaceFsmState.Inactive)
            .Permit(RaceFsmTrigger.StartTriggered, RaceFsmState.Start)
            .Permit(RaceFsmTrigger.Toggle, RaceFsmState.Disabled)
            .OnEntry(_ => { InactiveOnEnter(); });
    }

    private void InactiveOnEnter()
    {
        _currentTriggerId = -1;
        UiTimer.Singleton._display = false;
        UiTimer.Singleton._active = false;

        for (int i = 0; i < Triggers.Count; i++)
        {
            if (i == 0) Triggers[i].MarkNext();
            else Triggers[i].Hide();
        }
    }
}