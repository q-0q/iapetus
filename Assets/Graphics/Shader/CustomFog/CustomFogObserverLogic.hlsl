uniform float4 _CustomFogObservers[64];
uniform int _CustomFogObserverCount;

void CalculateClosestCustomFogObserver_float(float3 WorldPos, out float OutMask)
{

    float finalMask = 0;
    
    for (int i = 0; i < _CustomFogObserverCount; i++)
    {
        float3 p = _CustomFogObservers[i].xyz;
        // float radius = _CustomFogObservers[i].w;
        float radius = 25.0;
        
        float d = distance(WorldPos, p);
        float mask = 1.0 - smoothstep(radius * 0.9, radius, d);
        
        finalMask = max(finalMask, mask);
    }
    
    OutMask = finalMask;
}

