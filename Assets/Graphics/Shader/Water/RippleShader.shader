Shader "Unlit/RippleShader"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _PrevRT;
            sampler2D _CurrentRT;
            float4 _CurrentRT_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Inside RippleShader.shader
            float _Damping;

            float4 frag (v2f i) : SV_Target  
            {  
                float2 e = _CurrentRT_TexelSize.xy;  
                
                float p10 = tex2D(_CurrentRT, i.uv - float2(0, e.y)).r;  
                float p01 = tex2D(_CurrentRT, i.uv - float2(e.x, 0)).r;  
                float p21 = tex2D(_CurrentRT, i.uv + float2(e.x, 0)).r;  
                float p12 = tex2D(_CurrentRT, i.uv + float2(0, e.y)).r;  
                
                float p11 = tex2D(_PrevRT, i.uv).r;  

                // Laplacian filter for wave propagation
                float d = (p10 + p01 + p21 + p12) * 0.5 - p11;  
                
                // Apply dissipation
                d *= _Damping; 
                
                return float4(d, 0, 0, 1);  
            }
            
            ENDCG
        }
    }
}
