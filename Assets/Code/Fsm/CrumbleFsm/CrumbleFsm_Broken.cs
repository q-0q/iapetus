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
                transform.DOShakePosition(0.5f, 0.3f);
                _collider.enabled = false;
                _renderer.enabled = false;
            });
    }
}