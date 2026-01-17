public partial class TrialCollectibleFsm
{
    private void ReadyConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Ready)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredStartingZone, TrialCollectibleFsmState.Start);
    }
}