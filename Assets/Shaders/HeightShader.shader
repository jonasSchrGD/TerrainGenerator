Shader "Terrain/HeightShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        static const int maxheights = 30;

        int colorCount;
        float heigths[maxheights];
        float3 colors[maxheights];

        struct Input
        {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            for (int i = 0; i < colorCount; i++)
            {
                float saturation = saturate(sign(IN.worldPos.y - heigths[i]));
                o.Albedo = o.Albedo * (1 - saturation) + saturation * colors[i];
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
