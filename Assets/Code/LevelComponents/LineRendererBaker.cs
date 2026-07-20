using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LineRendererBaker : MonoBehaviour
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
        if (!TryGetComponent(out LineRenderer lineRenderer)) return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RecordObject(lineRenderer, "Bake Line Renderer");
        }
#endif

        Vector3[] localPositions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
            {
                localPositions[i] = transform.InverseTransformPoint(points[i].position);
            }
        }

        // Configure the LineRenderer to use local space positions
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = localPositions.Length;
        lineRenderer.SetPositions(localPositions);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(lineRenderer);
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