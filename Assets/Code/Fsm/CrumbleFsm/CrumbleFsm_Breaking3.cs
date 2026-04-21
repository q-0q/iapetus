using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    private void Breaking3OnUpdate()
    {
        _renderer.material.SetFloat("_Glow", Mathf.Lerp(0, .7f, Mathf.InverseLerp(0.35f, 1f, TimeInCurrentState())));
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
                _renderer.transform.DOComplete();
                _renderer.material.SetFloat("_CrackAmount", 0.95f);
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(eventPath3), gameObject);
                WorldspaceShake(_renderer.transform, 0.3f, 0.3f);
                WorldspaceShake(_renderer.transform, 2f, 0.2f);
            });
    }
}