using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class LinearSplineBaker : MonoBehaviour
{
    public Transform pointsParent;
    public bool autoBake = false;

    // Cache to track positions and detect changes
    private readonly List<Vector3> _lastPositions = new List<Vector3>();
    
    private void Update()
    {
        if (!autoBake || pointsParent == null) return;

        var points = GetPointsList();

        // Only bake if the hierarchy or positions actually changed
        if (HavePointsChanged(points))
        {
            Bake(points);
            UpdatePositionCache(points);
        }
    }

    private void Bake(List<Transform> points)
    {
        if (points == null || points.Count == 0) return;
        if (!TryGetComponent(out SplineContainer splineContainer)) return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RecordObject(splineContainer, "Bake Linear Spline");
        }
#endif

        var spline = splineContainer.Spline;
        spline.Clear();

        Vector3[] localPositions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
            {
                localPositions[i] = transform.InverseTransformPoint(points[i].position);
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 forward = Vector3.forward;

            if (points.Count > 1)
            {
                if (i < points.Count - 1)
                {
                    forward = (localPositions[i + 1] - localPositions[i]).normalized;
                }
                else
                {
                    forward = (localPositions[i] - localPositions[i - 1]).normalized;
                }
            }

            if (forward == Vector3.zero) forward = Vector3.forward;

            Vector3 upVector = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(forward, upVector)) > 0.99f)
            {
                upVector = Vector3.right;
            }

            Quaternion knotRotation = Quaternion.LookRotation(forward, upVector);

            var knot = new BezierKnot()
            {
                Position = localPositions[i],
                Rotation = knotRotation,
                TangentIn = Vector3.zero,
                TangentOut = Vector3.zero
            };
            
            spline.Add(knot, TangentMode.Linear);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(splineContainer);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    private bool HavePointsChanged(List<Transform> points)
    {
        // If the count doesn't match, a point was added or removed
        if (points.Count != _lastPositions.Count) return true;

        // Check if any individual point has moved
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null) return true;
            if (points[i].position != _lastPositions[i]) return true;
        }

        return false;
    }

    private void UpdatePositionCache(List<Transform> points)
    {
        _lastPositions.Clear();
        foreach (var p in points)
        {
            if (p != null) _lastPositions.Add(p.position);
        }
    }

    private List<Transform> GetPointsList()
    {
        var points = new List<Transform>();
        if (pointsParent == null) return points;

        for (int i = 0; i < pointsParent.childCount; i++)
        {
            points.Add(pointsParent.GetChild(i));
        }

        return points;
    }
}