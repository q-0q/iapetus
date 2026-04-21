Shader "Unlit/SimpleVolumeShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Float) = 0.02
        _StepSize("Step Size", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Back
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_STEP_COUNT 64
            #define EPSILON 0.00001f

            float4 _Color;
            float _Alpha;
            float _StepSize;
            float4 _PlayerWorldPosition;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objectPos : TEXCOORD0;
                float3 rayDirObject : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 camObj = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;

                o.objectPos = v.vertex.xyz;
                o.rayDirObject = normalize(o.objectPos - camObj);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.vertex;

                return o;
            }

            float4 BlendUnder(float4 baseColor, float4 newColor)
            {
                baseColor.rgb += (1.0 - baseColor.a) * newColor.a * newColor.rgb;
                baseColor.a += (1.0 - baseColor.a) * newColor.a;
                return baseColor;
            }

            float InverseLerp(float a, float b, float value)
            {
                // Avoid division by zero
                if (a == b) return 0.0;
                return saturate((value - a) / (b - a)); // saturate clamps result between 0 and 1
            }
            
            // Smooth 3D value noise, returns 0..1
            float SmoothNoise3D(float3 p)
            {
                // Integer/fractional parts
                float3 pi = floor(p);
                float3 pf = frac(p);

                // Smoothstep for smooth interpolation
                float3 w = pf * pf * (3.0 - 2.0 * pf);

                // Simple hash function for grid corners
                #define HASH33(v) frac(sin(dot(v, float3(127.1, 311.7, 74.7))) * 43758.5453)

                // Corners
                float n000 = HASH33(pi + float3(0.0, 0.0, 0.0));
                float n001 = HASH33(pi + float3(0.0, 0.0, 1.0));
                float n010 = HASH33(pi + float3(0.0, 1.0, 0.0));
                float n011 = HASH33(pi + float3(0.0, 1.0, 1.0));
                float n100 = HASH33(pi + float3(1.0, 0.0, 0.0));
                float n101 = HASH33(pi + float3(1.0, 0.0, 1.0));
                float n110 = HASH33(pi + float3(1.0, 1.0, 0.0));
                float n111 = HASH33(pi + float3(1.0, 1.0, 1.0));

                // Trilinear interpolation
                float nx00 = lerp(n000, n100, w.x);
                float nx01 = lerp(n001, n101, w.x);
                float nx10 = lerp(n010, n110, w.x);
                float nx11 = lerp(n011, n111, w.x);

                float nxy0 = lerp(nx00, nx10, w.y);
                float nxy1 = lerp(nx01, nx11, w.y);

                float nxyz = lerp(nxy0, nxy1, w.z);

                return nxyz;
            }


            fixed4 frag(v2f i) : SV_Target
            {

                float3 surfacePos = i.objectPos;
                float3 surfaceWorldPos = mul(unity_ObjectToWorld, float4(surfacePos, 1.0)).xyz;
                float surfaceDistance = distance(surfaceWorldPos, _PlayerWorldPosition);
                
                
                float3 pos = i.objectPos;
                float3 rayDir = normalize(i.rayDirObject);
                float4 accumulatedColor = float4(0, 0, 0, 0);

                for (int step = 0; step < MAX_STEP_COUNT; step++)
                {
                    if (max(abs(pos.x), max(abs(pos.y), abs(pos.z))) < 0.5f + EPSILON)
                    {
                        float3 worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;
                        float4 clipPos = UnityWorldToClipPos(worldPos);
                        float2 screenUV = (clipPos.xy / clipPos.w) * 0.5 + 0.5;

                        #if UNITY_UV_STARTS_AT_TOP
                        screenUV.y = 1.0 - screenUV.y;
                        #endif

                        float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UNITY_PROJ_COORD(screenUV));
                        float linearSceneDepth = LinearEyeDepth(sceneDepth);
                        float linearRayDepth = LinearEyeDepth(clipPos.z / clipPos.w);

                        if (linearRayDepth < linearSceneDepth - 0.001)
                        {
                            
                            float4 sampleCol = _Color;
                            sampleCol.a *= _Alpha;
                            accumulatedColor = BlendUnder(accumulatedColor, sampleCol);
                            if (accumulatedColor.a >= 0.95)
                                break;
                        }

                        pos += rayDir * _StepSize;
                    }
                    else
                    {
                        break;
                    }
                }

                float fakeFogAlphaMod = clamp(0.0f, 1.0f, InverseLerp(150.0f, 20.0f, surfaceDistance));
                accumulatedColor.a *= fakeFogAlphaMod;
                accumulatedColor.rgb *= fakeFogAlphaMod;
                return accumulatedColor;
            }
            ENDCG
        }
    }

    FallBack Off
}
