uniform float4 _CustomFogObservers[64];
uniform int _CustomFogObserverCount;

void CalculateClosestCustomFogObserver_float(float3 WorldPos, out float3 OutPos)
{
    float3 closestPos;
    float closestDistance;
    bool init = true;

    for (int i = 0; i < _CustomFogObserverCount; i++)
    {
        float3 p = _CustomFogObservers[i].xyz;
        float d = distance(WorldPos, p);
        if (init)
        {
            closestPos = p;
            closestDistance = d;
            init = false;
        }
        else if (d < closestDistance)
        {
            closestPos = p;
            closestDistance = d;
        }        
    }

    OutPos = closestPos;
}

