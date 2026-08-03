uniform float4 _CustomPointLightPositions[64];
uniform float4 _CustomPointLightLerps[64];
uniform float4 _CustomPointLightColors[64];
uniform int _CustomPointLightCount;


float IL(float minVal, float maxVal, float value)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

void GetCustomPointLightColor_float(float3 WorldPos, float3 InputColor, out float3 OutColor)
{
    
    float3 finalColor = InputColor;
    
    for (int i = 0; i < _CustomPointLightCount; i++)
    {
        
        float3 lightPosition = float3(_CustomPointLightPositions[i].x, _CustomPointLightPositions[i].y, _CustomPointLightPositions[i].z);
        // float3 lightPosition = float3(0,0,0);
        float d = distance(WorldPos, lightPosition);
        float lerpMin = _CustomPointLightLerps[i].x;
        float lerpMax = _CustomPointLightLerps[i].y;
        float lerpPower = _CustomPointLightLerps[i].z;
        float lerp = IL(lerpMin, lerpMax, d);
        lerp = 1 - pow(lerp, lerpPower);

        float3 c = float3(1,0,0);
        float3 color = float3(_CustomPointLightColors[i].x, _CustomPointLightColors[i].y, _CustomPointLightColors[i].z) * lerp;

        
        finalColor += color;
    }
    
    OutColor = finalColor;
}


