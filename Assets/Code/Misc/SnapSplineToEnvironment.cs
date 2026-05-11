using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SplineContainer))]
[ExecuteAlways]
public class SnapSplineToEnvironment : MonoBehaviour
{
    private const KeyCode AlignmentKey = KeyCode.K;
    private const float RaycastDistance = 10f;
    private const float surfaceDistanceOffset = 0.25f;

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
        for (int i = 0; i < _splineContainer.Spline.Count; i++)
        {
            AlignKnotToSurface(i);
        }
    }

    private void AlignKnotToSurface(int index)
    {
        var spline = _splineContainer.Spline;
        var knot = spline[index];

        // 1. Calculate World Space Knot Properties
        Vector3 worldKnotPos = transform.TransformPoint(knot.Position);
    
        // Combine container rotation with knot rotation to get the world-space orientation
        Quaternion worldKnotRotation = transform.rotation * (Quaternion)knot.Rotation;
    
        // This is the "Local Down" of the knot in World Space
        Vector3 localDownDirection = worldKnotRotation * Vector3.down;

        // 2. Raycast from slightly 'above' the knot relative to its own orientation
        // We offset it back along its 'up' axis to ensure we don't start inside the geometry
        Ray ray = new Ray(worldKnotPos - (localDownDirection * RaycastDistance * 0.5f), localDownDirection);
        Debug.DrawRay(ray.origin, localDownDirection * RaycastDistance, Color.magenta, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, RaycastDistance, Fsm.GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            
            Debug.DrawRay(hit.point, Vector3.up, Color.yellow, 2f );
            // 3. Update Position
            knot.Position = transform.InverseTransformPoint(hit.point + hit.normal * surfaceDistanceOffset);

            // 4. Update Rotation
            // We want to keep the current Forward direction but align the Up to the surface normal
            Vector3 worldForward = worldKnotRotation * Vector3.forward;
            Vector3 worldNormal = hit.normal;

            // Project forward onto the surface plane to keep the path tangential to the ground
            Vector3 projectedForward = Vector3.ProjectOnPlane(worldForward, worldNormal);

            if (projectedForward.sqrMagnitude > 0.001f)
            {
                // Create the new world rotation
                Quaternion newWorldRot = Quaternion.LookRotation(projectedForward, worldNormal);
            
                // Convert the world rotation back to the Spline's local space
                knot.Rotation = Quaternion.Inverse(transform.rotation) * newWorldRot;
            }

            // 5. Apply changes back to the spline
            spline[index] = knot;
            
            // 3. EXPLICITLY MARK DIRTY (Crucial for Prefabs)
            EditorUtility.SetDirty(_splineContainer);
            
            // If this is a prefab, this ensures the override is recorded
            PrefabUtility.RecordPrefabInstancePropertyModifications(_splineContainer);
        }
    }


#endif
}