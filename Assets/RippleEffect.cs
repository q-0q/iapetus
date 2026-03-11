using System.Collections;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    [Header("Settings")]
    public int textureSize = 512;
    [Range(0.9f, 0.995f)]
    public float damping = 0.98f;
    
    [Header("Resources")]
    public Shader rippleShader;
    public Material rippleStampMaterial;

    private RenderTexture currRT, prevRT, tempRT;
    private Material rippleMat;
    private static readonly int PrevTexID = Shader.PropertyToID("_PrevRT");
    private static readonly int CurrTexID = Shader.PropertyToID("_CurrentRT");
    private static readonly int DampingID = Shader.PropertyToID("_Damping");
    private static readonly int CenterID = Shader.PropertyToID("_Center");

    void Start()
    {
        // 1. Initialize RenderTextures with Linear ReadWrite to avoid Gamma shifts
        currRT = CreateRT();
        prevRT = CreateRT();
        tempRT = CreateRT();

        rippleMat = new Material(rippleShader);

        // 2. Initial Clear
        ClearRT(currRT);
        ClearRT(prevRT);
        ClearRT(tempRT);

        // 3. Assign to the object's renderer
        GetComponent<Renderer>().material.SetTexture("_RippleTex", currRT);

        StartCoroutine(SimulationLoop());
    }

    RenderTexture CreateRT()
    {
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        rt.filterMode = FilterMode.Bilinear;
        rt.wrapMode = TextureWrapMode.Clamp; // Vital: Prevents edge-to-edge feedback explosion
        rt.Create();
        return rt;
    }

    void ClearRT(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(false, true, Color.clear); // Clears to 0,0,0,0
        RenderTexture.active = active;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddRipple(1.0f, 0.02f);
        }
    }

    IEnumerator SimulationLoop()
    {
        while (true)
        {
            // Set shader properties
            rippleMat.SetTexture(PrevTexID, prevRT);
            rippleMat.SetTexture(CurrTexID, currRT);
            rippleMat.SetFloat(DampingID, damping);

            // Calculate new state into tempRT
            Graphics.Blit(null, tempRT, rippleMat);

            // Cycle textures: Temp becomes Current, Current becomes Previous
            RenderTexture oldPrev = prevRT;
            prevRT = currRT;
            currRT = tempRT;
            tempRT = oldPrev; 

            // Update the display texture
            GetComponent<Renderer>().material.SetTexture("_RippleTex", currRT);

            yield return null;
        }
    }

    public void AddRipple(float strength, float radius)
    {
        Vector2 uv = new Vector2(0.5f, 0.5f); // Center of the screen
        rippleStampMaterial.SetVector(CenterID, new Vector4(uv.x, uv.y, strength, radius));

        // Stamp onto current state
        Graphics.Blit(currRT, tempRT, rippleStampMaterial);
        
        // Swap temp back into current so simulation picks it up
        RenderTexture t = currRT;
        currRT = tempRT;
        tempRT = t;
    }
}