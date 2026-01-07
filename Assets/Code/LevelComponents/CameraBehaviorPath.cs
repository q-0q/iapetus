using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviorPath : CameraBehaviorZone
{
    public List<Transform> keyFrames;

    public override Vector3 GetCameraForward(Vector3 inputPosition)
    {
        if (keyFrames == null || keyFrames.Count == 0)
            return Vector3.forward;

        if (keyFrames.Count == 1)
            return keyFrames[0].forward;

        // Find the two closest keyframes
        Transform closestA = null;
        Transform closestB = null;

        float distA = float.MaxValue;
        float distB = float.MaxValue;

        foreach (var kf in keyFrames)
        {
            float d = Vector3.Distance(inputPosition, kf.position);

            if (d < distA)
            {
                distB = distA;
                closestB = closestA;

                distA = d;
                closestA = kf;
            }
            else if (d < distB)
            {
                distB = d;
                closestB = kf;
            }
        }

        // Safety fallback
        if (closestA == null || closestB == null)
            return closestA != null ? closestA.forward : Vector3.forward;

        // Compute interpolation factor based on relative distance
        float t = distA / (distA + distB);
        t = SmoothLerp01(t);

        // Interpolate rotation and return forward
        Quaternion blendedRotation =
            Quaternion.Slerp(closestA.rotation, closestB.rotation, t);

        return blendedRotation * Vector3.forward;
    }

    public static float SmoothLerp01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}