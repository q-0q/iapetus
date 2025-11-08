using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    private void BrokenConfigure()
    {
        Machine.Configure(CrumbleFsmState.Broken)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Forming)
            .OnEntry(_ =>
            {
                transform.DOShakePosition(1f, 0.6f);
                _collider.enabled = false;
                _renderer.enabled = false;
            });
    }
}