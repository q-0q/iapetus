using Unity.VisualScripting;
using UnityEngine;

public partial class CrumbleFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnPlayerParentTransformChanged(Transform t, float momentum, float yVelocity)
    {
        if (t != transform) return;
        Machine.Fire(CrumbleFsmTrigger.PlayerSetAsParent);
    }
}