Shader "Hidden/RippleStamp"
{
    Properties {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Center; // x,y = position, z = strength, w = radius

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // 1. Read the EXISTING height from the source texture (currRT)
                float existingHeight = tex2D(_MainTex, i.uv).r;

                // 2. Calculate the local ripple influence
                float dist = distance(i.uv, _Center.xy);
                
                // Use a sharp falloff so the effect is strictly local
                // If dist is large, 'ripple' becomes 0.0 very quickly
                float ripple = exp(-dist * dist / (_Center.w * _Center.w));

                // 3. Add the new ripple to the old height
                float newHeight = existingHeight + (ripple * _Center.z);

                // 4. Return ONLY the R channel, ensuring no weird offsets
                return float4(newHeight, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}