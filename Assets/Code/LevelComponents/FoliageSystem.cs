using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class FoliageSystem : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    [Header("Placement")]
    [Tooltip("Blades per square meter")]
    public float density = 4f;

    public int maxInstances = 10000;
    public float raycastDepth = 50f;
    public float raycastOriginYOffset = 1f;

    [Header("Randomization")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public Vector2 rotationYRange = new Vector2(0f, 360f);
    public Vector2 offsetRange = new Vector2(-0.4f, 0.4f);

    [Header("Layers")]
    public LayerMask receiveFoliageMask;

    Matrix4x4[] instData;
    RenderParams rp;

    void Start()
    {
        rp = new RenderParams(material)
        {
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = false
        };

        BuildInstances();
    }

    void BuildInstances()
    {
        Collider col = GetComponent<Collider>();
        Bounds localBounds = GetLocalColliderBounds(col);

        Vector3 lossy = transform.lossyScale;

        float worldArea =
            localBounds.size.x * lossy.x *
            localBounds.size.z * lossy.z;

        int targetCount = Mathf.Min(
            Mathf.RoundToInt(worldArea * density),
            maxInstances
        );

        List<Matrix4x4> matrices = new List<Matrix4x4>(targetCount);

        for (int i = 0; i < targetCount; i++)
        {
            // Sample in LOCAL space
            Vector3 localPoint = new Vector3(
                Random.Range(localBounds.min.x, localBounds.max.x),
                localBounds.max.y,
                Random.Range(localBounds.min.z, localBounds.max.z)
            );
            
            Vector3 localOffset = new Vector3(
                Random.Range(offsetRange.x, offsetRange.y),
                0f,
                Random.Range(offsetRange.x, offsetRange.y)
            );

            // Convert to world
            Vector3 worldOrigin =
                transform.TransformPoint(localPoint) + localOffset +
                transform.up * raycastOriginYOffset;

            // Ray direction now fully respects GameObject rotation
            Vector3 rayDirection = -transform.up;

            if (!Physics.Raycast(
                worldOrigin,
                rayDirection,
                out RaycastHit hit,
                raycastDepth,
                ~LayerMask.GetMask(),
                QueryTriggerInteraction.Ignore))
                continue;
            
            
            if (Vector3.Angle(hit.normal, -rayDirection) > 60f) continue;

            if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
                continue;

            if (RaycastCheckSphere(hit, rayDirection)) continue;
            
            // Random offset in object-local space

            Vector3 position = hit.point;

            // --------------------------------------------------
            // ROTATION: Align mesh opposite ray direction
            // --------------------------------------------------

            Vector3 foliageUp = -rayDirection;

            // Align mesh's +Y with foliageUp
            Quaternion alignToRay =
                Quaternion.FromToRotation(Vector3.up, foliageUp);

            // Random twist around ray axis
            float randomY = Random.Range(rotationYRange.x, rotationYRange.y);
            Quaternion twist =
                Quaternion.AngleAxis(randomY, foliageUp);

            Quaternion rotation = twist * alignToRay;

            float scale = Random.Range(scaleRange.x, scaleRange.y);

            matrices.Add(Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one * scale
            ));
        }
        
        Matrix4x4[] finalMatrices = matrices.ToArray();
        
        FoliageChunkManager.Instance.RegisterFoliage(mesh, material, finalMatrices);
        
    }

    private bool RaycastCheckSphere(RaycastHit hitInfo, Vector3 rayDirection)
    {
        var height = 300f;
        var gap = 1f;
        var origin = hitInfo.point + Vector3.up * height;
        
        // raycast both up and down to check for overhangs
        if (!Physics.Raycast(origin, Vector3.down, height - gap, Fsm.GetEnvironmentalLayermask(),
                QueryTriggerInteraction.Ignore)) return false;
        
        return !Physics.Raycast(hitInfo.point + Vector3.up, Vector3.up, height - gap, Fsm.GetEnvironmentalLayermask(),
                QueryTriggerInteraction.Ignore);
    }

    Bounds GetLocalColliderBounds(Collider col)
    {
        if (col is BoxCollider box)
        {
            return new Bounds(box.center, box.size);
        }
        else if (col is MeshCollider meshCol && meshCol.sharedMesh != null)
        {
            return meshCol.sharedMesh.bounds;
        }

        // Fallback
        Bounds b = col.bounds;
        Vector3 center = transform.InverseTransformPoint(b.center);
        Vector3 size = Vector3.Scale(
            b.size,
            new Vector3(
                1f / transform.lossyScale.x,
                1f / transform.lossyScale.y,
                1f / transform.lossyScale.z
            )
        );

        return new Bounds(center, size);
    }

    private float edgeThreshold = 0.1f;
    
    bool IsNearEdge(RaycastHit hit)
    {
        // barycentricCoordinate returns a Vector3 (u, v, w)
        // where x=u, y=v, and z=w
        Vector3 bary = hit.barycentricCoordinate;

        // Check if any coordinate is close to zero
        if (bary.x < edgeThreshold || bary.y < edgeThreshold || bary.z < edgeThreshold)
        {
            return true;
        }

        return false;
    }
}
