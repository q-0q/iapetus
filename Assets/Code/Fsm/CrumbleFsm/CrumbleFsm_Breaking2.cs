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
            .OnEntry(_ =>
            {
                _renderer.material.SetFloat("_CrackAmount", 0.45f);
                transform.DOShakePosition(0.3f, 0.5f);
            });
    }
}