using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    private void Breaking2OnUpdate()
    {
        // _renderer.material.SetFloat("_CrackAmount", Mathf.Lerp(0, 4f, Mathf.InverseLerp(0, 2f, TimeInCurrentState())));
    }

    private void Breaking2Configure()
    {
        Machine.Configure(CrumbleFsmState.Breaking2)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Breaking3)
            .PermitIf(FsmTrigger.Timeout, CrumbleFsmState.Idle, _ => PlayerFsm.Singleton.parentTransform != transform, 1)
            .OnEntry(_ =>
            {
                _crumbleParticleSystem.Play();
                _renderer.material.SetFloat("_CrackAmount", 0.95f);
                _renderer.transform.DOComplete();
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(eventPath2), gameObject);
                WorldspaceShake(_renderer.transform, 0.15f, 0.2f);
                WorldspaceShake(_renderer.transform, 1f, 0.15f);
            });
    }
}