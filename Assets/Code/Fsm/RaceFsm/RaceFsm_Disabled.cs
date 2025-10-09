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
        foreach (var t in Triggers)
        {
            t.Hide();
        }
    }
}