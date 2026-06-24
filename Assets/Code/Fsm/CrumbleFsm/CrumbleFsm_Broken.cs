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
                _renderer.material.SetFloat("_Glow", 1f);
                _renderer.material.SetFloat("_CrackAmount", 1.5f);
                _renderer.transform.DOComplete();
                WorldspaceShake(0.5f, 1.25f);
                WorldspaceShake(2f, 0.4f);
                _collider.enabled = false;
                _renderer.enabled = false;
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(eventPath4), gameObject);
            });
    }
}