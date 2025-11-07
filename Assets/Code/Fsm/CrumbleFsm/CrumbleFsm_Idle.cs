using DG.Tweening;

public partial class CrumbleFsm
{
    private void IdleConfigure()
    {
        Machine.Configure(CrumbleFsmState.Idle)
            .Permit(CrumbleFsmTrigger.PlayerSetAsParent, CrumbleFsmState.Breaking)
            .OnEntryFrom(FsmTrigger.Timeout, _ =>
            {
                transform.DOShakePosition(0.5f, 0.5f);
                _collider.enabled = true;
                _renderer.enabled = true;
            });
    }
}