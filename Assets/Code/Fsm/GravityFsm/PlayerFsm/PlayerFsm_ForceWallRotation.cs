using UnityEngine;

public partial class PlayerFsm
{
    private void ForceWallRotationOnUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit,
                ForceWallRotationRaycastDistance * GetRaycastTimeModifier(), ~0, QueryTriggerInteraction.Ignore))
        {
            var quaternion = Quaternion.LookRotation(-hit.normal, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion,
                RotationSpeed * Time.deltaTime * ForceWallRotationSpeed);
        }
    }
}