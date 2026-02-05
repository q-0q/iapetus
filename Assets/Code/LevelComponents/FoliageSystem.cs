using UnityEngine;
using UnityEngine.Rendering;

public class FoliageSystem : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    const int numInstances = 100;

    [Header("Randomization")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    public Vector2 offsetRange = new Vector2(-0.4f, 0.4f);
    public Vector2 rotationYRange = new Vector2(0f, 360f);

    Matrix4x4[] instData;
    RenderParams rp;

    void Start()
    {
        rp = new RenderParams(material)
        {
            shadowCastingMode = ShadowCastingMode.On,
            receiveShadows = true
        };

        instData = new Matrix4x4[numInstances * numInstances];

        int id = 0;

        for (int i = 0; i < numInstances; ++i)
        {
            for (int j = 0; j < numInstances; ++j)
            {
                // Base grid position
                Vector3 basePos = new Vector3(-4.5f + i, 0.0f, -4.5f + j);

                // Random position offset
                Vector3 offset = new Vector3(
                    Random.Range(offsetRange.x, offsetRange.y),
                    0f,
                    Random.Range(offsetRange.x, offsetRange.y)
                );

                Vector3 finalPos = basePos + offset;

                // Random Y rotation
                Quaternion rotation = Quaternion.Euler(
                    0f,
                    Random.Range(rotationYRange.x, rotationYRange.y),
                    0f
                );

                // Random uniform scale
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                Vector3 finalScale = Vector3.one * scale;

                instData[id++] = Matrix4x4.TRS(finalPos, rotation, finalScale);
            }
        }
    }

    void Update()
    {
        Graphics.RenderMeshInstanced(rp, mesh, 0, instData);
    }
}