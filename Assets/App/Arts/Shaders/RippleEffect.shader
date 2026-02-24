Shader "Custom/RippleEffect"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Base", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_GradTex);
            SAMPLER(sampler_GradTex);

            float4 _Params1; 
            float4 _Params2; 
            float4 _Reflection;
            float4 _Drop1, _Drop2, _Drop3; 

            float GetWave(float2 uv, float2 dropPos, float time)
            {
                if (time <= 0 || time > 1.2) return 0; 

                float2 dVec = (uv - dropPos);
                dVec.x *= _Params1.x; 
                float d = length(dVec);
                
                float t = time - d * _Params1.z;
                
                if (t < 0 || t > 1.0) return 0;

                float val = SAMPLE_TEXTURE2D_LOD(_GradTex, sampler_GradTex, float2(t, 0), 0).a;
                return (val - 0.5) * 2.0;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                float w = GetWave(uv, _Drop1.xy, _Drop1.z) +
                          GetWave(uv, _Drop2.xy, _Drop2.z) +
                          GetWave(uv, _Drop3.xy, _Drop3.z);

                float2 dx = float2(0.005, 0);
                float2 dy = float2(0, 0.005);
                
                float wX = GetWave(uv + dx, _Drop1.xy, _Drop1.z) + GetWave(uv + dx, _Drop2.xy, _Drop2.z) + GetWave(uv + dx, _Drop3.xy, _Drop3.z);
                float wY = GetWave(uv + dy, _Drop1.xy, _Drop1.z) + GetWave(uv + dy, _Drop2.xy, _Drop2.z) + GetWave(uv + dy, _Drop3.xy, _Drop3.z);
                float2 dw = float2(wX - w, wY - w);

                float2 duv = dw * _Params2.x * 0.15;
                float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + duv);

                float fr = pow(saturate(length(dw) * 3.0 * _Params2.y), 3.0);
                return lerp(col, _Reflection, fr);
            }
            ENDHLSL
        }
    }
}