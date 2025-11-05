using Code.TriggerParams;
using UnityEngine;

public abstract partial class GravityFsm
{
    private void GroundedOnUpdate()
    {
        YVelocity = 0;
        if (GetGroundedRaycastHit(out var hit))
        {
            var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * GroundedYPositionLerpStrength);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            GroundForwardSlope = Vector3.Angle(transform.forward, hit.normal);
            if (hit.transform != _parentTransform)
            {
                _parentTransform = hit.transform;
                _previousParentTransformPosition = _parentTransform.position;
                _previousParentRotation = _parentTransform.rotation;
                OnParentTransformChanged(_parentTransform);
            }
            print("WAAA base");
        }
        UpdateYVelocityMetadata();
    }

    private void GroundedConfigure()
    {
        Machine.Configure(GravityFsmState.Grounded)
            .SubstateOf(GravityFsmState.RespectParentTransform);
    }
}