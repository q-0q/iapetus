using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(SplineContainer))]
public class SplineWindEffect : MonoBehaviour
{
    private SplineContainer splineContainer;
    
    public float windStrength = 2f;
    public float windSpeed = 1f;

    private Quaternion knot0RotationBase;
    private Quaternion knot1RotationBase;
    
    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        knot0RotationBase = splineContainer.Spline[0].Rotation;
        knot1RotationBase = splineContainer.Spline[1].Rotation;
    }

    void Update()
    {
        var spline = splineContainer.Spline;
        AnimateTangent(spline, 0, knot0RotationBase);
        AnimateTangent(spline, 1, knot1RotationBase);
    }

    void AnimateTangent(Spline spline, int knotIndex, Quaternion rotationBase)
    {
        BezierKnot knot = spline[knotIndex];
        var rotation = rotationBase * Quaternion.Euler(Mathf.Sin(Time.time * windSpeed) * windStrength, Mathf.Sin(Time.time * windSpeed + 1f) * windStrength, 0);
        knot.Rotation = rotation;
        spline[knotIndex] = knot;
    }
}