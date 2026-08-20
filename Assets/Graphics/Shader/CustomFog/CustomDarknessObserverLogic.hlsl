uniform float4 _CustomDarknessObservers[64];
uniform int _CustomDarknessObserverCount;
uniform float _CustomDarknessWeight;

float IL2(float minVal, float maxVal, float value)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

void CalculateClosestCustomDarknessObserver_float(float4 WorldPos, float3 WorldNormal, float NormalInfluence, out float OutMask)
{
    float finalMask = 1.0;
    float3 normWorldNormal = normalize(WorldNormal);
    
    for (int i = 0; i < _CustomDarknessObserverCount; i++)
    {
        float4 p = _CustomDarknessObservers[i].xyzw;
        float mask = 1.0;
        
        if (p.w > 0.0)
        {
            float depth = length(WorldPos.xyz - p.xyz);
            float depthFactor = pow(IL2(0.0, p.w, depth), 0.5);

            float3 lightDir = normalize(p.xyz - WorldPos.xyz);
            float NdotL = saturate(dot(normWorldNormal, lightDir));
            NdotL = IL2(0, 0.1, NdotL);
            NdotL = lerp(1.0, NdotL, 0);
            
            // Calculate light intensity (1 = lit, 0 = dark)
            float lightIntensity = (1.0 - depthFactor) * NdotL;
            
            // Convert back to a darkness mask (0 = lit, 1 = dark)
            mask = 1.0 - lightIntensity;
        }
        
        finalMask = min(finalMask, saturate(mask));
    }
    
    OutMask = finalMask * _CustomDarknessWeight;
}