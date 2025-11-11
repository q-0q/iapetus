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
                _breakParticleSystem.Play();
                // _crumbleParticleSystem.Play();
                _renderer.material.SetFloat("_Glow", 0.1f);
                _renderer.material.SetFloat("_CrackAmount", 1.5f);
                transform.DOShakePosition(1f, 0.6f);
                _collider.enabled = false;
                _renderer.enabled = false;
            });
    }
}