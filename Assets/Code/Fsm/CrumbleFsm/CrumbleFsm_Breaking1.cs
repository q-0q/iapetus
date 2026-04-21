using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    
    private void Breaking1Configure()
    {
        Machine.Configure(CrumbleFsmState.Breaking1)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Breaking2)
            .PermitIf(FsmTrigger.Timeout, CrumbleFsmState.Idle, _ => PlayerFsm.Singleton.parentTransform != transform, 1)
            .OnEntry(_ =>
            {
                _crumbleParticleSystem.Play();
                _renderer.material.SetFloat("_CrackAmount", 0.25f);
                _renderer.transform.DOComplete();
                
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(eventPath1), gameObject);
                WorldspaceShake(_renderer.transform, 0.15f, 0.3f);
                WorldspaceShake(_renderer.transform, 1f, 0.15f);
            });
    }
}