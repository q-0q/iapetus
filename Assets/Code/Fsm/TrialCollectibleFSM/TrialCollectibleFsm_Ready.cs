public partial class TrialCollectibleFsm
{
    private void ReadyConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Ready)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredStartingZone, TrialCollectibleFsmState.Start);

        Machine.Configure(TrialCollectibleFsmState.ReadyUntaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
        
        Machine.Configure(TrialCollectibleFsmState.ReadyTaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
    }
}