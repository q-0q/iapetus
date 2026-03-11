using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WaterRippleSimulationRedux : MonoBehaviour
{
    public int TextureSize = 512;
    public RenderTexture CurrRT, TempRT;
    
    public Material rippleStampMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //Creating render textures and materials
        CurrRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        TempRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        
        CurrRT.Create();
        TempRT.Create();
        
        ClearRT(CurrRT);
        ClearRT(TempRT);
        
        //Change the texture in the material of this object to the render texture calculated by the ripple shader.
        Shader.SetGlobalTexture("_PlayerRippleSimulationTexture", CurrRT);
        ;
    }
    
    void ClearRT(RenderTexture rt)
    {
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = prev;
    }

    
    public void AddRipple(float strength = 1f, float radius = 0.5f)
    {
        Vector2 uv = Vector2.one * 0.5f;

        rippleStampMaterial.SetVector("_Center",
            new Vector4(uv.x, uv.y, strength, radius));

        Graphics.Blit(CurrRT, TempRT, rippleStampMaterial);
        Swap(ref CurrRT, ref TempRT);
    }
    
    void Swap(ref RenderTexture a, ref RenderTexture b)
    {
        RenderTexture t = a;
        a = b;
        b = t;
    }
    
    
}