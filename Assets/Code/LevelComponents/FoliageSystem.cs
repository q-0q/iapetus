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
    public float yOffset = 0f;

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

        int numSlices = Mathf.Min(
            Mathf.RoundToInt(worldArea * density),
            maxInstances
        );

        int slice = 0;

        List<Matrix4x4> matrices = new List<Matrix4x4>(maxInstances);

        while (slice < numSlices && matrices.Count < maxInstances)
        {
            
            // find base worldspace origin for slice
            
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

            Vector3 worldOrigin =
                transform.TransformPoint(localPoint) + localOffset +
                transform.up * raycastOriginYOffset;

            Vector3 rayDirection = -transform.up;

            var keepPiercing = true;

            while (keepPiercing && matrices.Count < maxInstances)
            {
                TryPlace(matrices, worldOrigin, rayDirection, out keepPiercing, out Vector3 newWorldOrigin);
                worldOrigin = newWorldOrigin;
            }

            slice++;

        }
        
        Matrix4x4[] finalMatrices = matrices.ToArray();
        
        if (transformBake) FoliageChunkManager.Instance.RegisterTransformFoliage(transform, mesh,material, finalMatrices);
        else FoliageChunkManager.Instance.RegisterFoliage(mesh, material, finalMatrices);
        
        // NewMethod(numSlices, localBounds, matrices);
    }
    
    private void TryPlace(List<Matrix4x4> matrices, Vector3 worldOrigin, Vector3 rayDirection, out bool keepPiercing, out Vector3 newWorldOrigin)
    {
        
        // if we completely whiff, exit and end piercing
        newWorldOrigin = worldOrigin;
        keepPiercing = false;
        
        if (!Physics.Raycast(
                worldOrigin,
                rayDirection,
                out var hit,
                raycastDepth,
                receiveFoliageMask | LayerMask.GetMask("FoliageMask"),
                QueryTriggerInteraction.Collide)) return;
        
        

        
        // if we hit foliage mask, exit and end piercing
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("FoliageMask"))
        {
            Debug.DrawRay(hit.point, Vector3.up * 5f, Color.red, 10f);
            return;
        }
        
        // from now on, keep piercing if we fail placement
        keepPiercing = true;
        newWorldOrigin = hit.point - (Vector3.up * 0.1f);
        
        // if it's too steep, dont place
        if (Vector3.Angle(hit.normal, -rayDirection) > maxSlope) return;
        
        // wrong layer or foliage mask, don't place
        if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0) return;
        
        // try to detect if we are inside geometry.
        // we reason by saying if there is an upwards facing normal above us
        // but not a downwards facing normal above us, then skip placement
        if (RaycastCheckSphere(hit, rayDirection)) return;
        
        // edge detection
        var edgeDelta = Mathf.Lerp(2f, 10f, Mathf.InverseLerp(Vector3.Angle(hit.normal, Vector3.up), 0f, 30f));
        if (IsNearEdge(worldOrigin, rayDirection, hit.distance + edgeDelta)) return;
        
        // done: create matrix and add to list
        
        Vector3 foliageUp = -rayDirection;
        Quaternion alignToRay =
            Quaternion.FromToRotation(Vector3.up, foliageUp);

        float randomY = Random.Range(rotationYRange.x, rotationYRange.y);
        Quaternion twist =
            Quaternion.AngleAxis(randomY, foliageUp);

        Quaternion rotation = twist * alignToRay;

        float scale = Random.Range(scaleRange.x, scaleRange.y);
        Vector3 position = hit.point;
        position += Vector3.up * yOffset;
            
        matrices.Add(Matrix4x4.TRS(
            position,
            rotation,
            Vector3.one * scale
        ));
        
    }
    
    private bool Test(Vector3 worldOrigin, Vector3 rayDirection, out RaycastHit hit)
    {
        
        if (!Physics.Raycast(
                worldOrigin,
                rayDirection,
                out hit,
                raycastDepth,
                receiveFoliageMask | LayerMask.GetMask("FoliageMask"),
                QueryTriggerInteraction.Ignore))
            return false;


        //
        if (Vector3.Angle(hit.normal, -rayDirection) > maxSlope) return false;

        if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
            return false;

        if (RaycastCheckSphere(hit, rayDirection)) return false;

        return true;
    }

    private void NewMethod(int numSlices, Bounds localBounds, List<Matrix4x4> matrices)
    {
        for (int i = 0; i < numSlices; i++)
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

            Vector3 worldOrigin =
                transform.TransformPoint(localPoint) + localOffset +
                transform.up * raycastOriginYOffset;

            Vector3 rayDirection = -transform.up;

            if (!Test(worldOrigin, rayDirection, out RaycastHit hit)) continue;

            var edgeDelta = Mathf.Lerp(2f, 10f, Mathf.InverseLerp(Vector3.Angle(hit.normal, Vector3.up), 0f, 30f));
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

            position += Vector3.up * yOffset;
            
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
        var gap = 0.01f;
        var origin = hitInfo.point + Vector3.up * height;
        
        // if (Physics.Raycast(origin, Vector3.down, height - gap, LayerMask.GetMask("FoliageMask"),
        //         QueryTriggerInteraction.Collide)) return true;


        if (Physics.Raycast(origin, Vector3.down, out RaycastHit downwardsHit, height - gap,
                Fsm.GetEnvironmentalLayermask(),
                QueryTriggerInteraction.Ignore))
        {
            if (Physics.Raycast(hitInfo.point + Vector3.up * gap, Vector3.up, out RaycastHit upwardsHit, height - gap, Fsm.GetEnvironmentalLayermask(),
                    QueryTriggerInteraction.Ignore))
            {
                if (upwardsHit.point.y < downwardsHit.point.y) return false;
            };

            return true;
        };

        return false;
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



}
