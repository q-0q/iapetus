using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    private void Breaking3OnUpdate()
    {
        _renderer.material.SetFloat("_Glow", Mathf.Lerp(0, 0.05f, Mathf.InverseLerp(0.15f, 1f, TimeInCurrentState())));
        _renderer.material.SetFloat("_CrackAmount", Mathf.Lerp(0.9f, 1.5f, Mathf.InverseLerp(0.15f, 1f, TimeInCurrentState())));
    }

    private void Breaking3Configure()
    {
        Machine.Configure(CrumbleFsmState.Breaking3)
            .PermitIf(FsmTrigger.Timeout, CrumbleFsmState.Idle, _ => PlayerFsm.Singleton.parentTransform != transform, 1)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Broken)
            .OnEntry(_ =>
            {
                _crumbleParticleSystem.Play();
                transform.DOShakePosition(0.3f, 0.2f);
                transform.DOShakePosition(2f, 0.1f);
            });
    }
}