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
    public float raycastHeight = 50f;

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

        // -----------------------------
        // WORLD-SPACE DENSITY FIX
        // -----------------------------
        Vector3 lossy = transform.lossyScale;

        float worldArea =
            localBounds.size.x * lossy.x *
            localBounds.size.z * lossy.z;

        int targetCount = Mathf.Min(
            Mathf.RoundToInt(worldArea * density),
            maxInstances
        );
        // -----------------------------

        List<Matrix4x4> matrices = new List<Matrix4x4>(targetCount);

        for (int i = 0; i < targetCount; i++)
        {
            // Sample in LOCAL space
            Vector3 localPoint = new Vector3(
                Random.Range(localBounds.min.x, localBounds.max.x),
                localBounds.max.y,
                Random.Range(localBounds.min.z, localBounds.max.z)
            );

            Vector3 worldOrigin =
                transform.TransformPoint(localPoint) +
                transform.up * raycastHeight;

            if (!Physics.Raycast(
                worldOrigin,
                -transform.up,
                out RaycastHit hit,
                raycastHeight * 2f,
                ~LayerMask.GetMask(),
                QueryTriggerInteraction.Ignore))
                continue;

            if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
                continue;

            Vector3 localOffset = new Vector3(
                Random.Range(offsetRange.x, offsetRange.y),
                0f,
                Random.Range(offsetRange.x, offsetRange.y)
            );

            Vector3 position =
                hit.point + transform.TransformDirection(localOffset);

            Quaternion rotation =
                Quaternion.AngleAxis(
                    Random.Range(rotationYRange.x, rotationYRange.y),
                    hit.normal
                );

            float scale = Random.Range(scaleRange.x, scaleRange.y);

            matrices.Add(Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one * scale
            ));
        }

        instData = matrices.ToArray();
    }

    void Update()
    {
        if (instData == null || instData.Length == 0)
            return;

        Graphics.RenderMeshInstanced(rp, mesh, 0, instData);
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

        // Fallback (least accurate)
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
