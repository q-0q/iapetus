uniform float4 _CustomDarknessObservers[64];
uniform int _CustomDarknessObserverCount;

uniform float _CustomDarknessWeight;

float IL2(float minVal, float maxVal, float value)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

void CalculateClosestCustomDarknessObserver_float(float4 WorldPos, out float OutMask)
{
    
    float finalMask = 1.0;
    
    for (int i = 0; i < _CustomDarknessObserverCount; i++)
    {
        
        float4 p = _CustomDarknessObservers[i].xyzw;
        float mask = 1.0;
        if (p.w > 0.0)
        {
            float depth = length(WorldPos.xyz - p.xyz);
            float depthFactor = pow(IL2(0, p.w, depth), 0.5);
            mask = depthFactor;
        }
        finalMask = min(finalMask, saturate(mask));
    }
    
    OutMask = finalMask * _CustomDarknessWeight;
}


