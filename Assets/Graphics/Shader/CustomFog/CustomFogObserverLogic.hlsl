uniform float4 _CustomFogObservers[64];
uniform int _CustomFogObserverCount;

uniform float _CustomFogYMin;
uniform float _CustomFogYMax;
uniform float _CustomFogYPower;
uniform float _CustomFogMinimumYFactor;

uniform float _CustomFogYAddMin;
uniform float _CustomFogYAddMax;
uniform float _CustomFogYAddPower;
uniform float _CustomFogYAddClamp;

uniform float _CustomFogDepthMin;
uniform float _CustomFogDepthMax;
uniform float _CustomFogDepthPower;

uniform float _CustomFogSkyboxLift;

float InverseLerp(float minVal, float maxVal, float value)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

void CalculateClosestCustomFogObserver_float(float3 WorldPos, out float OutMask)
{

    float finalMask = 1.0;
    
    for (int i = 0; i < _CustomFogObserverCount; i++)
    {
        float3 p = _CustomFogObservers[i].xyz;

        float relativeY = WorldPos.y - p.y;
        
        float yFactor = 1.0 - pow(InverseLerp(_CustomFogYMin, _CustomFogYMax, relativeY), _CustomFogYPower);
        yFactor = max(yFactor, _CustomFogMinimumYFactor);
        
        float yAddFactor = 1.0 - pow(InverseLerp(_CustomFogYAddMin, _CustomFogYAddMax, relativeY), _CustomFogYAddPower);
        yAddFactor = min(yAddFactor, _CustomFogYAddClamp);

        float depth = length(WorldPos.xz - p.xz);
        float depthFactor = pow(InverseLerp(_CustomFogDepthMin, _CustomFogDepthMax, depth), _CustomFogDepthPower);


        float skyboxLiftFactor = pow(InverseLerp(_CustomFogSkyboxLift, _CustomFogSkyboxLift * -0.5, relativeY), 0.75);
        skyboxLiftFactor = max(skyboxLiftFactor, _CustomFogMinimumYFactor);
        
        
        // float mask = yAddFactor;
        float mask = saturate(lerp(lerp(yAddFactor, yFactor, depthFactor), skyboxLiftFactor, InverseLerp(1000.0, 1100.0, depth)));
        
        finalMask = min(finalMask, saturate(mask));
    }
    
    OutMask = finalMask;
}


