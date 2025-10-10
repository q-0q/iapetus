using Code.TriggerParams;
using UnityEngine;

public abstract partial class GravityFsm
{
    private void RespectParentTrasnformConfigure()
    {
        Machine.Configure(GravityFsmState.RespectParentTransform)
            .OnEntry(@params =>
            {
                if (@params is not RaycastHitParam p) return;
                motionParentTransform = p.Hit.transform;
                print(motionParentTransform.name);
            });
    }
}