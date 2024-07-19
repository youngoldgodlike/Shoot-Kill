Shader "Custom/RedEdges"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUV = i.uv * _ScreenParams.xy;
                float distFromCenter = distance(screenUV, 0.5);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                if (distFromCenter > 0.5)
                {
                    col = fixed4(1, 0, 0, 1); // Красный цвет на краях экрана
                }
                
                return col;
            }
            ENDCG
        }
    }
}