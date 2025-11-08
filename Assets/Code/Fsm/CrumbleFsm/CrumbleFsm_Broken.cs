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
                _renderer.material.SetFloat("_Glow", 0f);
                _renderer.material.SetFloat("_CrackAmount",0);
                transform.DOShakePosition(1f, 0.6f);
                _collider.enabled = false;
                _renderer.enabled = false;
            });
    }
}