using DG.Tweening;

public partial class CrumbleFsm
{

    private void BreakingConfigure()
    {
        Machine.Configure(CrumbleFsmState.Breaking)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Broken)
            .OnEntry(_ =>
            {
                transform.DOShakePosition(0.5f, 0.5f);
            });
    }
}