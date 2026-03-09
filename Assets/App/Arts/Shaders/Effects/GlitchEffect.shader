Shader "Custom/GlitchEffect"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        Pass
        {
            Name "GlitchEffect"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest  Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float2 _ScanLineJitter;
            float  _HorizontalShake;
            float2 _ColorDrift;

            float nrand(float x, float y)
            { return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453); }

            half4 Fragment(Varyings i) : SV_Target
            {
                float u = i.texcoord.x;
                float v = i.texcoord.y;

                float jitter = (nrand(v, _Time.x) * 2 - 1);
                jitter *= step(_ScanLineJitter.y, abs(jitter)) * _ScanLineJitter.x;
                float shake = (nrand(_Time.x, 2) - 0.5) * _HorizontalShake;
                float drift = sin(v + _ColorDrift.y) * _ColorDrift.x;

                float2 uv1 = frac(float2(u + jitter + shake,           v));
                float2 uv2 = frac(float2(u + jitter + shake + drift,    v));

                half4 s1 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, uv1);
                half4 s2 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, uv2);

                half4 rgba = half4(s1.r, s2.g, s1.b, 1.0) * max(s1.a, s2.a);

                return rgba;
            }
            ENDHLSL
        }
    }
}
