using DG.Tweening;

public partial class CrumbleFsm
{
    private void IdleConfigure()
    {
        Machine.Configure(CrumbleFsmState.Idle)
            .Permit(CrumbleFsmTrigger.PlayerSetAsParent, CrumbleFsmState.Breaking1)
            .OnEntryFrom(FsmTrigger.Timeout, _ =>
            {
                _renderer.material.SetFloat("_CrackAmount",0);
                transform.DOShakePosition(0.5f, 0.5f);
                _collider.enabled = true;
                _renderer.enabled = true;
            });
    }
}