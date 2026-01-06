using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviorPath : CameraBehaviorZone
{
    public List<Transform> keyFrames;

    public override Vector3 GetCameraForward(Vector3 inputPosition)
    {
        if (keyFrames == null || keyFrames.Count < 2)
            return Vector3.zero;

        // Track the two closest samples
        float bestDist1 = float.PositiveInfinity;
        float bestDist2 = float.PositiveInfinity;

        Vector3 forward1 = Vector3.zero;
        Vector3 forward2 = Vector3.zero;

        for (int i = 0; i < keyFrames.Count - 1; i++)
        {
            Vector3 a = keyFrames[i].position;
            Vector3 b = keyFrames[i + 1].position;

            Vector3 ab = b - a;
            float abSqrMag = ab.sqrMagnitude;

            if (abSqrMag < Mathf.Epsilon)
                continue;

            float t = Vector3.Dot(inputPosition - a, ab) / abSqrMag;
            t = Mathf.Clamp01(t);

            Vector3 closestPoint = a + t * ab;
            float dist = Vector3.Distance(inputPosition, closestPoint);

            Quaternion interpolatedRotation = Quaternion.Slerp(
                keyFrames[i].rotation,
                keyFrames[i + 1].rotation,
                t);

            Vector3 forward = interpolatedRotation * Vector3.forward;

            // Insert into best two
            if (dist < bestDist1)
            {
                bestDist2 = bestDist1;
                forward2 = forward1;

                bestDist1 = dist;
                forward1 = forward;
            }
            else if (dist < bestDist2)
            {
                bestDist2 = dist;
                forward2 = forward;
            }
        }

        // If only one valid point was found
        if (bestDist2 == float.PositiveInfinity)
            return forward1.normalized;

        // Distance-based weighting (inverse distance)
        const float epsilon = 0.0001f;
        float w1 = 1f / (bestDist1 + epsilon);
        float w2 = 1f / (bestDist2 + epsilon);

        Vector3 blendedForward =
            (forward1 * w1 + forward2 * w2) / (w1 + w2);

        return blendedForward.normalized;
    }
}
