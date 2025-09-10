Shader "Hidden/OutlineMergedComposite"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Thickness("Thickness", Range(0,20)) = 1.0
        _MinDepth("MinDepth", Range(0,1)) = 0.0
        _MaxDepth("MaxDepth", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }

        // PASS 0: compute outline mask into alpha channel
        Pass
        {
            Name "DepthToMask"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
            float _Thickness;
            float _MinDepth;
            float _MaxDepth;

            struct Attributes { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float2 texel = 1.0 / _ScreenParams.xy;

                float offsetPos = ceil(_Thickness * 0.5) * texel.x;
                float offsetNeg = -floor(_Thickness * 0.5) * texel.x;

                float d0 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(offsetNeg, offsetNeg)).r;
                float d1 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(offsetPos, offsetPos)).r;
                float d2 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(offsetPos, offsetNeg)).r;
                float d3 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(offsetNeg, offsetPos)).r;

                float d = length(float2(d1 - d0, d3 - d2));
                d = smoothstep(_MinDepth, _MaxDepth, d);

                return float4(0, 0, 0, d); // store mask in alpha
            }
            ENDHLSL
        }

        // PASS 1: composite outline over camera
        Pass
        {
            Name "Composite"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _OutlineColor;

            struct Attributes { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float mask = original.a; // outline stored in alpha from previous pass

                float3 result = lerp(original.rgb, _OutlineColor.rgb, mask);
                return float4(result, original.a);
            }
            ENDHLSL
        }
    }
}
