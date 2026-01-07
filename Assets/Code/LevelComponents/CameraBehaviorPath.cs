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

        float closestSqrDistance = float.PositiveInfinity;
        int bestSegmentIndex = 0;
        float bestT = 0f;

        // Find the closest sequential segment
        for (int i = 0; i < keyFrames.Count - 1; i++)
        {
            Vector3 a = keyFrames[i].position;
            Vector3 b = keyFrames[i + 1].position;
            Vector3 ab = b - a;

            float abSqrMag = ab.sqrMagnitude;
            if (abSqrMag < Mathf.Epsilon)
                continue;

            // Project inputPosition onto the segment
            float t = Vector3.Dot(inputPosition - a, ab) / abSqrMag;
            t = Mathf.Clamp01(t);

            Vector3 closestPoint = a + ab * t;
            float sqrDistance = (inputPosition - closestPoint).sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                bestSegmentIndex = i;
                bestT = t;
            }
        }

        // Smooth the interpolation parameter
        float smoothT = SmoothLerp01(bestT);

        // Blend rotations and return forward direction
        Quaternion rotA = keyFrames[bestSegmentIndex].rotation;
        Quaternion rotB = keyFrames[bestSegmentIndex + 1].rotation;

        Quaternion blendedRotation = Quaternion.Slerp(rotA, rotB, smoothT);
        return blendedRotation * Vector3.forward;
    }


    public static float SmoothLerp01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}