Shader "Hidden/WakeFade"
{
    Properties {
        _MainTex ("Texture", 2D) = "black" {}
        _WakeFade ("Fade Amount", Float) = 0.95
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
            float _WakeFade;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float currentVal = tex2D(_MainTex, i.uv).r;
                return float4(currentVal * _WakeFade, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}