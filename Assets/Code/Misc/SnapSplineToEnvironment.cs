using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SplineContainer))]
[ExecuteAlways]
public class SnapSplineToEnvironment : MonoBehaviour
{
    private const KeyCode AlignmentKey = KeyCode.P;
    private const float RaycastDistance = 10f;

    private SplineContainer _splineContainer;

#if UNITY_EDITOR
    private void OnEnable()
    {
        // SceneView.duringSceneGui allows us to catch keyboard events 
        // specifically when the mouse is over the Scene View.
        SceneView.duringSceneGui += OnSceneGUI;
        TryGetComponent(out _splineContainer);
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // 1. Only run if this specific object is the active selection
        if (Selection.activeGameObject != gameObject) return;

        // 2. Check if we are in Play Mode
        if (Application.isPlaying) return;

        Event e = Event.current;

        // 3. Check for the key. 
        // We check for KeyDown/KeyUp or if the key is held during a Repaint/Layout event
        if (e != null && e.keyCode == AlignmentKey)
        {
            // Only align if the key is pressed down or being held
            if (e.type == EventType.KeyDown || e.type == EventType.Used)
            {
                AlignKnotsToSurface();
                
                // Optional: Force the scene to repaint so the movement looks smooth
                sceneView.Repaint();
            }
        }
    }

    private void AlignKnotsToSurface()
    {
        Undo.RecordObject(transform, "Align to Surface");

        for (int i = 0; i < _splineContainer.Spline.Count; i++)
        {
            AlignKnotToSurface(i);
        }
        
        EditorUtility.SetDirty(transform);
    }

    private void AlignKnotToSurface(int index)
    {
        var knot = _splineContainer.Spline[index];
        var knotPosition = transform.TransformPoint(new Vector3(knot.Position.x, knot.Position.y, knot.Position.z));
        var direction = transform.TransformDirection(((Quaternion)knot.Rotation) * Vector3.down);
        
        Ray ray = new Ray(knotPosition - direction * 0.5f, direction);
        Debug.DrawRay(knotPosition, direction, Color.magenta, 10f);
        
        if (Physics.Raycast(ray, out RaycastHit hit, RaycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            
            knot.Position = transform.InverseTransformPoint(hit.point);
            knot.Rotation = Quaternion.LookRotation((Quaternion)knot.Rotation * Vector3.forward, transform.InverseTransformDirection(hit.normal));
            _splineContainer.Spline[index] = knot;
        }
    }


#endif
}