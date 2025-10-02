using UnityEngine;

public abstract partial class GravityFsm
{
    private void GroundedOnUpdate()
    {
        YVelocity = 0;
        if (GetGroundedRaycastHit(out var hit))
        {
            var f = 50f;
            var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * f);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}