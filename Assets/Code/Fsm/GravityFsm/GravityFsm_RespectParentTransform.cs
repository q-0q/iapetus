using Code.TriggerParams;
using UnityEngine;

public abstract partial class GravityFsm
{
    private void RespectParentTransformOnUpdate()
    {
        if (_parentTransform == null) return;
        var posDiff = _parentTransform.position - _previousParentTransformPosition;
        transform.position += posDiff;
        _previousParentTransformPosition = _parentTransform.position;
    }
    
    private void RespectParentTransformConfigure()
    {
        Machine.Configure(GravityFsmState.RespectParentTransform)
            .OnEntry(@params =>
            {
                if (@params is not RaycastHitParam p) return;
                print("entry");
                _parentTransform = p.Hit.transform;
                _previousParentTransformPosition = _parentTransform.position;
                print(_parentTransform.name);
            });
    }
}