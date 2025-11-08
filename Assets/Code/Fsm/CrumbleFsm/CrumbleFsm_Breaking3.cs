using DG.Tweening;
using UnityEngine;

public partial class CrumbleFsm
{

    private void Breaking3OnUpdate()
    {
        // _renderer.material.SetFloat("_CrackAmount", Mathf.Lerp(0, 4f, Mathf.InverseLerp(0, 2f, TimeInCurrentState())));
    }

    private void Breaking3Configure()
    {
        Machine.Configure(CrumbleFsmState.Breaking3)
            .Permit(FsmTrigger.Timeout, CrumbleFsmState.Broken)
            .OnEntry(_ =>
            {
                _renderer.material.SetFloat("_CrackAmount", 0.9f);
                transform.DOShakePosition(0.3f, 0.5f);
            });
    }
}