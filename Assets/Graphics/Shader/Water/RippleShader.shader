Shader "Unlit/RippleShader"
{
    Properties
    {
        _Damping ("Damping", Float) = 0.98
        _Speed ("Wave Speed", Range(0.0, 0.5)) = 0.5
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
            float _Damping;
            float _Speed; // New Speed parameter

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target  
            {  
                float2 e = _CurrentRT_TexelSize.xy;  
                
                // Sample neighbors from the current state
                float p10 = tex2D(_CurrentRT, i.uv + float2(0, e.y)).r;  
                float p01 = tex2D(_CurrentRT, i.uv + float2(e.x, 0)).r;  
                float p21 = tex2D(_CurrentRT, i.uv - float2(e.x, 0)).r;  
                float p12 = tex2D(_CurrentRT, i.uv - float2(0, e.y)).r;  
                
                // Sample the previous state
                float p11_prev = tex2D(_PrevRT, i.uv).r;  
                float p11_curr = tex2D(_CurrentRT, i.uv).r;

                // Wave Equation: NewValue = (NeighborSum * Speed) - PreviousValue
                // Note: If Speed > 0.5, the energy increases every frame and the simulation "explodes".
                float d = (p10 + p01 + p21 + p12) * _Speed - p11_prev;  
                
                // Apply damping to lose energy over time
                d *= _Damping; 
                
                // Clamp slightly to prevent infinite precision artifacts (optional)
                return float4(d, 0, 0, 1);  
            }
            
            ENDCG
        }
    }
}