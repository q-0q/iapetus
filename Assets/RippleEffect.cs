using System.Collections;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    [Header("Ripple Settings")]
    public int textureSize = 512;
    [Range(0.9f, 0.995f)] public float damping = 0.98f;
    public float speed = 0.4f;
    
    [Header("Wake Settings")]
    public float wakeDecaySpeed = 2.0f; // Higher = faster fade
    
    
    [Header("Resources")]
    public Shader rippleShader;
    public Shader wakeFadeShader; // New Shader for simple fading
    public Material rippleStampMaterial;
    
    [Header("Movement Settings")]
    public Transform playerTransform;
    public float worldSize = 20f;
    private Vector3 lastPlayerPos;
    private Material offsetMat;

    private RenderTexture currRT, prevRT, tempRT, wakeRT, wakeTempRT;
    private Material rippleMat, wakeMat;
    
    private static readonly int PrevTexID = Shader.PropertyToID("_PrevRT");
    private static readonly int CurrTexID = Shader.PropertyToID("_CurrentRT");
    private static readonly int DampingID = Shader.PropertyToID("_Damping");
    private static readonly int SpeedID = Shader.PropertyToID("_Speed");
    private static readonly int CenterID = Shader.PropertyToID("_Center");
    private static readonly int WakeFadeID = Shader.PropertyToID("_WakeFade");

    void Start()
    {
        playerTransform = transform.parent;
        
        currRT = CreateRT();
        prevRT = CreateRT();
        tempRT = CreateRT();
        wakeRT = CreateRT(); // Initialize Wake Texture
        wakeTempRT = CreateRT(); // Create the second temp buffer

        rippleMat = new Material(rippleShader);
        wakeMat = new Material(wakeFadeShader);
        offsetMat = new Material(Shader.Find("Hidden/RippleOffset"));

        ClearRT(currRT);
        ClearRT(prevRT);
        ClearRT(tempRT);
        ClearRT(wakeRT);
        ClearRT(wakeTempRT);

        // Set Globals for Shader Graph
        Shader.SetGlobalTexture("_PlayerRippleSimulationTexture", currRT);
        Shader.SetGlobalTexture("_PlayerWakeTexture", wakeRT);
        
        lastPlayerPos = playerTransform.position;
        StartCoroutine(SimulationLoop());
    }

    RenderTexture CreateRT()
    {
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        rt.filterMode = FilterMode.Bilinear;
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.Create();
        return rt;
    }

    void ClearRT(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(false, true, Color.clear);
        RenderTexture.active = active;
    }

    IEnumerator SimulationLoop()
    {
        while (true)
        {
            ApplyWorldShift();
        
            // --- 1. Ripple Simulation ---
            rippleMat.SetTexture(PrevTexID, prevRT);
            rippleMat.SetTexture(CurrTexID, currRT);
            rippleMat.SetFloat(DampingID, damping);
            rippleMat.SetFloat(SpeedID, speed);
        
            Graphics.Blit(null, tempRT, rippleMat);

            // Cycle Ripple textures
            RenderTexture oldPrev = prevRT;
            prevRT = currRT;
            currRT = tempRT;
            tempRT = oldPrev; 

            // --- 2. Wake Simulation ---
            float frameFade = Mathf.Pow(0.5f, Time.deltaTime * wakeDecaySpeed);
            wakeMat.SetFloat(WakeFadeID, frameFade);
        
            Graphics.Blit(wakeRT, wakeTempRT, wakeMat);
        
            RenderTexture wTemp = wakeRT;
            wakeRT = wakeTempRT;
            wakeTempRT = wTemp;

            // --- 3. Update Globals ---
            Shader.SetGlobalTexture("_PlayerRippleSimulationTexture", currRT);
            Shader.SetGlobalTexture("_PlayerWakeTexture", wakeRT);

            yield return null;
        }
    }

    public void AddRipple(float strength, float radius)
    {
        Vector2 uv = new Vector2(0.5f, 0.5f);
        rippleStampMaterial.SetVector(CenterID, new Vector4(uv.x, uv.y, strength, radius));

        // Stamp Ripple (Uses Ripple's temp)
        Graphics.Blit(currRT, tempRT, rippleStampMaterial);
        RenderTexture t = currRT; currRT = tempRT; tempRT = t;
    }
    
    public void AddWake(float strength, float radius)
    {
        Vector2 uv = new Vector2(0.5f, 0.5f);
        rippleStampMaterial.SetVector(CenterID, new Vector4(uv.x, uv.y, strength, radius));
        
        // Stamp Wake (Uses Wake's temp)
        rippleStampMaterial.SetVector(CenterID, new Vector4(uv.x, uv.y, strength * 6f, radius * 5f));
        Graphics.Blit(wakeRT, wakeTempRT, rippleStampMaterial);
        RenderTexture w = wakeRT; wakeRT = wakeTempRT; wakeTempRT = w;
    }

    // Update Shift to be generic so it doesn't accidentally use the Ripple temp for the Wake
        void ShiftTexture(RenderTexture rt, RenderTexture buffer)
        {
            Graphics.Blit(rt, buffer, offsetMat);
            Graphics.Blit(buffer, rt); 
        }
    
    void ApplyWorldShift()
    {
        Vector3 delta = playerTransform.position - lastPlayerPos;
        if (delta.sqrMagnitude < 0.0001f) return;

        Vector2 uvOffset = new Vector2(delta.x / worldSize, delta.z / worldSize);
        offsetMat.SetVector("_Offset", uvOffset);

        ShiftTexture(currRT, tempRT);
        ShiftTexture(prevRT, tempRT);
        ShiftTexture(wakeRT, wakeTempRT); // Use wakeTempRT for the wake shift

        lastPlayerPos = playerTransform.position;
    }

    void ShiftTexture(RenderTexture rt)
    {
        Graphics.Blit(rt, tempRT, offsetMat);
        Graphics.Blit(tempRT, rt); 
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerRippleGenerated += AddRipple;
        PlayerFsm.OnPlayerWakeGenerated += AddWake;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerRippleGenerated -= AddRipple;
        PlayerFsm.OnPlayerWakeGenerated -= AddWake;
    }
}