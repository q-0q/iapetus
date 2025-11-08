using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{
    private void IdleOnUpdate()
    {
        
        _renderer.material.SetFloat("_Glow", Mathf.Lerp(_renderer.material.GetFloat("_Glow"), 0, Time.deltaTime * 10f));
        _renderer.material.SetFloat("_CrackAmount", Mathf.Lerp(_renderer.material.GetFloat("_CrackAmount"), 0, Time.deltaTime * 10f));
    }
    
    
    private void IdleConfigure()
    {
        Machine.Configure(CrumbleFsmState.Idle)
            .Permit(CrumbleFsmTrigger.PlayerSetAsParent, CrumbleFsmState.Breaking1)
            .OnEntryFrom(FsmTrigger.Timeout, _ =>
            {

                // transform.DOShakePosition(0.6f, 0.2f);
                _collider.enabled = true;
                _renderer.enabled = true;
            });
    }
}