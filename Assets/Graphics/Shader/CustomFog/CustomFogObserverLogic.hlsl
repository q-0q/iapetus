uniform float4 _CustomFogObservers[64];
uniform int _CustomFogObserverCount;

uniform float _CustomFogYMin;
uniform float _CustomFogYMax;
uniform float _CustomFogYPower;
uniform float _CustomFogMinimumYFactor;

uniform float _CustomFogAltYMin;
uniform float _CustomFogAltYMax;
uniform float _CustomFogAltYPower;
uniform float _CustomFogMaximumAltYFactor;

uniform float _CustomFogYAddMin;
uniform float _CustomFogYAddMax;
uniform float _CustomFogYAddPower;
uniform float _CustomFogYAddClamp;

uniform float _CustomFogDepthMin;
uniform float _CustomFogDepthMax;
uniform float _CustomFogDepthPower;
uniform float _CustomFogDepthClamp;

uniform float _CustomFogSkyboxLift;

float InverseLerp(float minVal, float maxVal, float value)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

void CalculateClosestCustomFogObserver_float(float4 WorldPos, out float OutMask)
{

    
    float finalMask = 1.0;
    
    for (int i = 0; i < _CustomFogObserverCount; i++)
    {
        float4 p = _CustomFogObservers[i].xyzw;

        float relativeY = WorldPos.y - p.y;
        float mask = 1.0;

        if (p.w > 0.0)
        {
            float depth = length(WorldPos.xyz - p.xyz);
            float depthFactor = pow(InverseLerp(_CustomFogDepthMin, _CustomFogDepthMax * p.w, depth), _CustomFogDepthPower);
            mask = depthFactor;
            // mask = 0;
        }

        else
        {
            float yFactor = 1.0 - pow(InverseLerp(_CustomFogYMin, _CustomFogYMax, relativeY), _CustomFogYPower);
            yFactor = max(yFactor, _CustomFogMinimumYFactor);

            float altYFactor = pow(InverseLerp(_CustomFogAltYMin, _CustomFogAltYMax, relativeY), _CustomFogAltYPower);
            altYFactor = min(altYFactor, _CustomFogMaximumAltYFactor);
            
            float yAddFactor = 1.0 - pow(InverseLerp(_CustomFogYAddMin, _CustomFogYAddMax, relativeY), _CustomFogYAddPower);
            yAddFactor = min(yAddFactor, _CustomFogYAddClamp);

            float depth = length(WorldPos.xz - p.xz);
            float depthFactor = pow(InverseLerp(_CustomFogDepthMin, _CustomFogDepthMax, depth), _CustomFogDepthPower);
            depthFactor = min(depthFactor, _CustomFogDepthClamp);


            float skyboxLiftFactor = pow(InverseLerp(_CustomFogSkyboxLift, _CustomFogSkyboxLift * -0.5, relativeY), 0.75);
            skyboxLiftFactor = max(skyboxLiftFactor, _CustomFogMinimumYFactor);
            
            mask = saturate(lerp(depthFactor + yFactor + altYFactor + yAddFactor, skyboxLiftFactor, InverseLerp(1000.0, 1100.0, depth)));
            // mask = yFactor;
        }
        
        finalMask = min(finalMask, saturate(mask));
    }
    
    OutMask = finalMask;
}


