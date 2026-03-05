public partial class OnetimeSwitchFsm
{

    private void OnConfigure()
    {
        Machine.Configure(OnetimeSwitchFsmState.On)
            .OnEntry(_ =>
            {
                _interactable.SetEnabled(false);
                SaveSystem.WritePersistentEvent(persistentEvent, 0);
                OnOnetimeSwitchFsmTurnedOn?.Invoke(this);
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(_eventPath), gameObject);
            });
    }
}