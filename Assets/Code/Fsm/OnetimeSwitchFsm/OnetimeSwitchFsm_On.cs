public partial class OnetimeSwitchFsm
{

    private void OnConfigure()
    {
        Machine.Configure(OnetimeSwitchFsmState.On)
            .OnEntry(_ =>
            {
                _interactable.SetEnabled(false);
                OnOnetimeSwitchFsmTurnedOn?.Invoke(this);
            });
    }
}