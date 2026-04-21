// This sits OUTSIDE the function, so 'uniform' is allowed
uniform float4 _Points[64];
uniform int _PointCount;

void CalculateMultiPointMask_float(float3 WorldPos, float DistanceOffset, out float OutMask)
{
    float finalMask = 0;

    for (int i = 0; i < _PointCount; i++)
    {
        float3 p = _Points[i].xyz;
        float radius = _Points[i].w; 
        
        float d = distance(WorldPos, p) + DistanceOffset;
        float mask = 1.0 - smoothstep(radius * 0.9, radius, d);
        
        finalMask = max(finalMask, mask);
    }

    OutMask = finalMask;
}

