using Code.TriggerParams;
using UnityEngine;

public abstract partial class GravityFsm
{


    private void RespectParentTransformOnUpdate()
    {
        if (_parentTransform == null) return;

        // Move position by the parent's movement delta
        Vector3 posDiff = _parentTransform.position - _previousParentTransformPosition;
        transform.position += ComputeCollisionMove(posDiff);

        // Compute rotation delta
        Quaternion rotationDelta = _parentTransform.rotation * Quaternion.Inverse(_previousParentRotation);

        // Rotate position around the parent
        Vector3 offset = transform.position - _parentTransform.position;
        offset = rotationDelta * offset;
        var rotationMove = (_parentTransform.position + offset) - transform.position;
        transform.position += ComputeCollisionMove(rotationMove);

        
        // Apply only the yaw (rotation around Vector3.up), discard pitch and roll
        Vector3 forward = rotationDelta * transform.forward;
        forward.y = 0; // Flatten to horizontal plane
        if (forward.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        // Update previous transform state
        _previousParentTransformPosition = _parentTransform.position;
        _previousParentRotation = _parentTransform.rotation;
    }

    
    private void RespectParentTransformConfigure()
    {
        Machine.Configure(GravityFsmState.RespectParentTransform)
            .OnEntry(@params =>
            {
                if (@params is not RaycastHitParam p) return;
                _parentTransform = p.Hit.transform;
                _previousParentTransformPosition = _parentTransform.position;
                _previousParentRotation = _parentTransform.rotation;
            });
    }

}