public partial class TrialCollectibleFsm
{

    private void ActiveConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Active)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredEndingZone, TrialCollectibleFsmState.Complete);
    }
}