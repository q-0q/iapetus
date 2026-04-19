using UnityEngine;

[ExecuteAlways]
public class CustomFogController : MonoBehaviour
{
    
    public Color Color = Color.white;
    
    public float YMin = -100f;
    public float YMax = 100f;
    public float YPower = 2f;
    
    public float DepthMin = 20f;
    public float DepthMax = 100f;
    public float DepthPower = 2f;
    
    public float NoiseSubtractionAmount = 50f;
    public float NoiseAScale = 0.02f;
    public Vector3 NoiseAVelocity = new Vector3(2, 2, 2);
    public float NoiseBScale = 0.01f;
    public Vector3 NoiseBVelocity = new Vector3(-1, -1, -1);

    // Update is called once per frame
    public void Update()
    {
        Shader.SetGlobalColor("_CustomFogColor", Color);
            
        Shader.SetGlobalFloat("_CustomFogYMin", YMin);
        Shader.SetGlobalFloat("_CustomFogYMax", YMax);
        Shader.SetGlobalFloat("_CustomFogYPower", YPower);
            
        Shader.SetGlobalFloat("_CustomFogDepthMin", DepthMin);
        Shader.SetGlobalFloat("_CustomFogDepthMax", DepthMax);
        Shader.SetGlobalFloat("_CustomFogDepthPower", DepthPower);
            
        Shader.SetGlobalFloat("_CustomFogNoiseSubtractionAmount", NoiseSubtractionAmount);
        Shader.SetGlobalFloat("_CustomFogNoiseAScale", NoiseAScale);
        Shader.SetGlobalFloat("_CustomFogNoiseBScale", NoiseBScale);
            
        Shader.SetGlobalVector("_CustomFogNoiseAVelocity", NoiseAVelocity);
        Shader.SetGlobalVector("_CustomFogNoiseBVelocity", NoiseBVelocity);
    }

}
