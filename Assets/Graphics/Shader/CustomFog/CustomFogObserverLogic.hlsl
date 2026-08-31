uniform float4 _CustomFogObservers[64];
uniform int _CustomFogObserverCount;

uniform float _CustomFogNearLowerBlanketMin;
uniform float _CustomFogNearLowerBlanketMax;
uniform float _CustomFogNearLowerBlanketPower;
uniform float _CustomFogMaxNearLowerBlanketFactor;

uniform float _CustomFogFarLowerBlanketMin;
uniform float _CustomFogFarLowerBlanketMax;
uniform float _CustomFogFarLowerBlanketPower;
uniform float _CustomFogMaxFarLowerBlanketFactor;

uniform float _CustomFogLowerBlanketDistanceMin;
uniform float _CustomFogLowerBlanketDistanceMax;
uniform float _CustomFogLowerBlanketDistancePower;

uniform float _CustomFogUpperBlanketMin;
uniform float _CustomFogUpperBlanketMax;
uniform float _CustomFogUpperBlanketPower;
uniform float _CustomFogMaxUpperBlanketFactor;

uniform float _CustomFogDepthMin;
uniform float _CustomFogDepthMax;
uniform float _CustomFogDepthPower;
uniform float _CustomFogMaxDepthFactor;

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
            

            float sphereDepth = length(WorldPos.xyz - p.xyz);
            float depthFactor = pow(InverseLerp(_CustomFogDepthMin, _CustomFogDepthMax, sphereDepth), _CustomFogDepthPower);
            depthFactor = min(depthFactor, _CustomFogMaxDepthFactor);

            float nearLowerBlankerFactor = 1.0 - pow(InverseLerp(_CustomFogNearLowerBlanketMin, _CustomFogNearLowerBlanketMax, relativeY), _CustomFogNearLowerBlanketPower);
            nearLowerBlankerFactor = min(nearLowerBlankerFactor, _CustomFogMaxNearLowerBlanketFactor);

            float farLowerBlanketFactor = 1.0 - pow(InverseLerp(_CustomFogFarLowerBlanketMin, _CustomFogFarLowerBlanketMax, relativeY), _CustomFogFarLowerBlanketPower);
            farLowerBlanketFactor = min(farLowerBlanketFactor, _CustomFogMaxFarLowerBlanketFactor);

            float circleDepth = length(WorldPos.xz - p.xz);
            float lowerBlanketDistanceFactor = pow(InverseLerp(_CustomFogLowerBlanketDistanceMin, _CustomFogLowerBlanketDistanceMax, circleDepth), _CustomFogLowerBlanketDistancePower);

            float lowerBlanketFactor = lerp(nearLowerBlankerFactor, farLowerBlanketFactor + nearLowerBlankerFactor, lowerBlanketDistanceFactor);

            float upperBlanketFactor = pow(InverseLerp(_CustomFogUpperBlanketMin, _CustomFogUpperBlanketMax, relativeY), _CustomFogUpperBlanketPower);
            upperBlanketFactor = min(upperBlanketFactor, _CustomFogMaxUpperBlanketFactor);


            float skyboxLiftFactor = pow(InverseLerp(_CustomFogSkyboxLift, _CustomFogSkyboxLift * -0.5, relativeY), 0.75);
            // skyboxLiftFactor = max(skyboxLiftFactor, _CustomFogMinimumYFactor);

            mask = saturate(depthFactor + lowerBlanketFactor + upperBlanketFactor);

            // mask = lowerBlanketDistanceFactor;
            
            // mask = saturate(lerp(depthFactor + yFactor + altYFactor + yAddFactor, skyboxLiftFactor, InverseLerp(1000.0, 1100.0, depth)));
            // mask = yFactor;
        }
        
        finalMask = min(finalMask, saturate(mask));
    }
    
    OutMask = finalMask;
}


