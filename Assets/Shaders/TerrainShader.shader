Shader "Terrain/TerrainShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct input
            {
                float3 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            static const int maxheights = 30;
            
            int colorCount;
            float heigths[maxheights];
            fixed4 colors[maxheights];

            v2f vert (input v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                
                o.color = fixed4(1,1,1,1);
                for (int i = 0; i < colorCount; i++)
                {
                    float saturation = saturate(sign(v.vertex.y - heigths[i]));
                    o.color = o.color * (1 - saturation) + saturation * fixed4(colors[i].rgb, 1);
                }

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.color *= nl * _LightColor0;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
