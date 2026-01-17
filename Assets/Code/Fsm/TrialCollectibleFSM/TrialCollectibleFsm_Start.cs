public partial class TrialCollectibleFsm
{

    private void StartConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Start)
            .Permit(TrialCollectibleFsmTrigger.PlayerExitedStartingZone, TrialCollectibleFsmState.Active);
    }
}