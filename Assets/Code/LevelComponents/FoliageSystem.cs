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
            shadowCastingMode = ShadowCastingMode.On,
            receiveShadows = true
        };

        BuildInstances();
    }

    void BuildInstances()
    {
        Collider col = GetComponent<Collider>();
        Bounds bounds = col.bounds;

        float area = bounds.size.x * bounds.size.z;
        int targetCount = Mathf.Min(
            Mathf.RoundToInt(area * density),
            maxInstances
        );

        List<Matrix4x4> matrices = new List<Matrix4x4>(targetCount);

        for (int i = 0; i < targetCount; i++)
        {
            // Random point inside collider bounds (XZ)
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 origin = new Vector3(x, bounds.max.y + raycastHeight, z);

            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, ~LayerMask.GetMask(), QueryTriggerInteraction.Ignore))
                continue;
            

            // If we hit something that is NOT ReceiveFoliage → skip
            if (((1 << hit.collider.gameObject.layer) & receiveFoliageMask) == 0)
                continue;
            
            // Random offset (small jitter)
            Vector3 offset = new Vector3(
                Random.Range(offsetRange.x, offsetRange.y),
                0f,
                Random.Range(offsetRange.x, offsetRange.y)
            );

            Vector3 position = hit.point + offset;

            Quaternion rotation = Quaternion.Euler(
                0f,
                Random.Range(rotationYRange.x, rotationYRange.y),
                0f
            );

            float scale = Random.Range(scaleRange.x, scaleRange.y);

            matrices.Add(
                Matrix4x4.TRS(position, rotation, Vector3.one * scale)
            );
        }

        instData = matrices.ToArray();
    }

    void Update()
    {
        if (instData == null || instData.Length == 0)
            return;

        Graphics.RenderMeshInstanced(rp, mesh, 0, instData);
    }
}
