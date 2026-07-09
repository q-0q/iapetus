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
    public float edgeDistance = 1f;
    public float maxSlope = 60f;

    [Header("Randomization")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public Vector2 rotationYRange = new Vector2(0f, 360f);
    public Vector2 offsetRange = new Vector2(-0.4f, 0.4f);

    [Header("Layers")]
    public LayerMask receiveFoliageMask;
    

    Matrix4x4[] instData;
    RenderParams rp;

    [Tooltip("If true, binds the foliage to the transform of the foliage system, for use in moving/rotating platforms. not performant, use sparingly")]
    public bool transformBake = false;

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

            if (!Test(worldOrigin, rayDirection, out RaycastHit hit)) continue;

            var edgeDelta = Mathf.Lerp(2f, 20f, Mathf.InverseLerp(Vector3.Angle(hit.normal, Vector3.up), 0f, 30f));
            if (IsNearEdge(worldOrigin, rayDirection, hit.distance + edgeDelta)) continue;

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
        
        if (transformBake) FoliageChunkManager.Instance.RegisterTransformFoliage(transform, mesh,material, finalMatrices);
        else FoliageChunkManager.Instance.RegisterFoliage(mesh, material, finalMatrices);
        
    }

    private bool RaycastCheckSphere(RaycastHit hitInfo, Vector3 rayDirection)
    {
        var height = 300f;
        var gap = 1f;
        var origin = hitInfo.point + Vector3.up * height;
        
        if (Physics.Raycast(origin, Vector3.down, height - gap, LayerMask.GetMask("FoliageMask"),
                QueryTriggerInteraction.Collide)) return true;

        if (Physics.Raycast(hitInfo.point + Vector3.up, Vector3.up, height - gap, Fsm.GetEnvironmentalLayermask(),
                QueryTriggerInteraction.Ignore))
        {
            // TODO: COMPUTE HEIGHT BASED ON UPWARDS RAYCAST POINT
            height = 10;
            origin = hitInfo.point + Vector3.up * height;
        };
        
        // raycast both up and down to check for overhangs
        return Physics.Raycast(origin, Vector3.down, height - gap, Fsm.GetEnvironmentalLayermask(),
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
    
    
    bool IsNearEdge(Vector3 worldOrigin, Vector3 rayDirection, float maxDistance)
    {

        var rayCount = 8;
        var originRadius = edgeDistance;
        

        for (int i = 0; i < rayCount; i++)
        {
            var origin = worldOrigin + Quaternion.Euler(0, 360f * ((float)i
                / rayCount), 0) * (Vector3.forward * originRadius);

            if (!Test(origin, rayDirection, out var hit)) return true;
            if (hit.distance >= maxDistance) return true;
        }

        return false;
    }

    private static bool CheckAllMasks(Vector3 position)
    {
        foreach (var mask in FoliageChunkManager.MaskSplines)
        {
            if (mask.MaskFoliageInstance(position)) return true;
        }

        return false;
    }


    private bool Test(Vector3 worldOrigin, Vector3 rayDirection, out RaycastHit hit)
    {
        if (!Physics.Raycast(
                worldOrigin,
                rayDirection,
                out hit,
                raycastDepth,
                ~LayerMask.GetMask("DeathCollider"),
                QueryTriggerInteraction.Ignore))
            return false;


        if (Vector3.Angle(hit.normal, -rayDirection) > maxSlope) return false;

        if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
            return false;

        if (RaycastCheckSphere(hit, rayDirection)) return false;

        return true;
    }
}
