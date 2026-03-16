using UnityEngine;

public class WaterRippleSimulationOld : MonoBehaviour
{
    [Header("Simulation")]
    public int resolution = 256;
    public float worldSize = 20f;
    public float damping = 0.995f;

    [Header("References")]
    public Transform player;
    public Material simulationMaterial;
    public Material rippleStampMaterial;
    public Material rippleScrollMaterial;

    RenderTexture heightA;
    RenderTexture heightB;
    RenderTexture temp;

    Vector2 simulationOrigin;
    float texelWorldSize;

    void Start()
    {
        heightA = CreateRT();
        heightB = CreateRT();
        temp = CreateRT();

        ClearRT(heightA);
        ClearRT(heightB);

        if (!player)
            player = transform.parent;

        simulationOrigin = new Vector2(player.position.x, player.position.z);

        texelWorldSize = worldSize / resolution;

        PushGlobals();
    }

    RenderTexture CreateRT()
    {
        RenderTexture rt = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RHalf);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;
        rt.Create();
        return rt;
    }

    void ClearRT(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = active;
    }

    void Update()
    {
        UpdateSimulationOrigin();
        SimulateRipples();
        PushGlobals();
    }

    void PushGlobals()
    {
        Shader.SetGlobalTexture("_PlayerRippleSimulationTexture", heightA);

        Shader.SetGlobalVector("_RippleSimulationOrigin",
            new Vector4(simulationOrigin.x, simulationOrigin.y, 0, 0));

        Shader.SetGlobalFloat("_RippleWorldSize", worldSize);
    }

    void UpdateSimulationOrigin()
    {
        Vector2 playerXZ = new Vector2(player.position.x, player.position.z);

        Vector2 delta = playerXZ - simulationOrigin;

        int shiftX = Mathf.FloorToInt(delta.x / texelWorldSize);
        int shiftY = Mathf.FloorToInt(delta.y / texelWorldSize);

        if (shiftX == 0 && shiftY == 0)
            return;

        simulationOrigin += new Vector2(
            shiftX * texelWorldSize,
            shiftY * texelWorldSize
        );

        ScrollTexture(shiftX, shiftY);
    }

    void ScrollTexture(int shiftX, int shiftY)
    {
        Vector2 uvOffset = new Vector2(
            -shiftX / (float)resolution,
            -shiftY / (float)resolution
        );

        rippleScrollMaterial.SetVector("_Offset", uvOffset);

        Graphics.Blit(heightA, temp, rippleScrollMaterial);
        Swap(ref heightA, ref temp);

        Graphics.Blit(heightB, temp, rippleScrollMaterial);
        Swap(ref heightB, ref temp);
    }

    void SimulateRipples()
    {
        simulationMaterial.SetFloat("_Damping", damping);
        simulationMaterial.SetTexture("_PrevTex", heightB);

        Graphics.Blit(heightA, temp, simulationMaterial);

        RenderTexture swap = heightB;
        heightB = heightA;
        heightA = temp;
        temp = swap;
    }

    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        RenderTexture t = a;
        a = b;
        b = t;
    }

    public void AddRipple(Vector3 worldPos, float strength = 1f, float radius = 0.5f)
    {
        Vector2 uv = WorldToRippleUV(worldPos);

        rippleStampMaterial.SetVector("_Center",
            new Vector4(uv.x, uv.y, strength, radius));

        Graphics.Blit(heightA, temp, rippleStampMaterial);
        Swap(ref heightA, ref temp);
    }

    Vector2 WorldToRippleUV(Vector3 worldPos)
    {
        Vector2 delta = new Vector2(
            worldPos.x - simulationOrigin.x,
            worldPos.z - simulationOrigin.y
        );

        delta /= worldSize;

        return delta + Vector2.one * 0.5f;
    }
}