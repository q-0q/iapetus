using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class BreakableConfig
{
    public Material Material;
    public Mesh Mesh;
    
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public Vector2 rotationYRange = new Vector2(0f, 360f);
    public Vector2 offsetRange = new Vector2(-0f, 0f);
    public float bitChance = 0;

    public EventReference EventReference;
}


[RequireComponent(typeof(Collider))]
public class BreakableSystem : MonoBehaviour
{
    [Header("Placement")]
    public float density = 4f;

    public int maxInstances = 100;
    public float raycastDepth = 50f;
    public float raycastOriginYOffset = 1f;
    
    public List<BreakableConfig> Configs;

    [Header("Layers")]
    public LayerMask receiveFoliageMask;

    // Matrix4x4[] instData;
    // RenderParams rp;

    void Start()
    {
        // rp = new RenderParams(material)
        // {
            // shadowCastingMode = ShadowCastingMode.Off,
            // receiveShadows = false
        // };

        BuildInstances();
    }

    void BuildInstances()
    {
        var objectPrefab = Resources.Load("Prefab/BreakableObject");
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

        // List<Matrix4x4> matrices = new List<Matrix4x4>(targetCount);

        for (int i = 0; i < targetCount; i++)
        {
            
            int configIndex = Random.Range(0, Configs.Count);
            
            
            // Sample in LOCAL space
            Vector3 localPoint = new Vector3(
                Random.Range(localBounds.min.x, localBounds.max.x),
                localBounds.max.y,
                Random.Range(localBounds.min.z, localBounds.max.z)
            );

            Vector3 localOffset = new Vector3(
                Random.Range(Configs[configIndex].offsetRange.x, Configs[configIndex].offsetRange.y),
                0f,
                Random.Range(Configs[configIndex].offsetRange.x, Configs[configIndex].offsetRange.y)
            );
            
            // Convert to world
            Vector3 worldOrigin =
                transform.TransformPoint(localPoint + localOffset) +
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

            if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
                continue;

            // Random offset in object-local space


            // Vector3 localOffset = Vector3.zero;
            Vector3 position = hit.point;

            // --------------------------------------------------
            // ROTATION: Align mesh opposite ray direction
            // --------------------------------------------------

            Vector3 foliageUp = -rayDirection;

            // Align mesh's +Y with foliageUp
            Quaternion alignToRay =
                Quaternion.FromToRotation(Vector3.up, foliageUp);

            // Random twist around ray axis
            float randomY = Random.Range(Configs[configIndex].rotationYRange.x, Configs[configIndex].rotationYRange.y);
            Quaternion twist =
                Quaternion.AngleAxis(randomY, foliageUp);

            Quaternion rotation = twist * alignToRay;

            float scale = Random.Range(Configs[configIndex].scaleRange.x, Configs[configIndex].scaleRange.y);

            var breakableObject = Object.Instantiate(objectPrefab, position, rotation, null) as GameObject;
            breakableObject.transform.localScale = Vector3.one * scale;
            breakableObject.transform.SetParent(transform);
            breakableObject.TryGetComponent(out BreakableObject component);
            component.Set(Configs[configIndex].Mesh, Configs[configIndex].Material, Configs[configIndex].EventReference, Configs[configIndex].bitChance);

            // matrices.Add(Matrix4x4.TRS(
            //     position,
            //     rotation,
            //     Vector3.one * scale
            // ));
        }
        
    }

    void Update()
    {

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
}
