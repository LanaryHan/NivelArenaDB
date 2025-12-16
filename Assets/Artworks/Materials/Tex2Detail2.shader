Shader "LK/URP/Transparent/Tex2Detail2"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Float) = 1

        _MainTex ("Main Tex", 2D) = "white" {}
        _USpeed ("USpeed", Float) = 0
        _VSpeed ("VSpeed", Float) = 0

        _AddTex ("Add Tex", 2D) = "white" {}
        _USpeed1 ("USpeed1", Float) = 0
        _VSpeed1 ("VSpeed1", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                half4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _Intensity;
                float4 _MainTex_ST;
                float4 _AddTex_ST;
                float _USpeed;
                float _VSpeed;
                float _USpeed1;
                float _VSpeed1;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_AddTex);
            SAMPLER(sampler_AddTex);

            float2 UVAnim(float2 uv, float uSpeed, float vSpeed)
            {
                float t = _Time.y;
                uv += float2(uSpeed, vSpeed) * t;
                return frac(uv);
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.uv, _AddTex);

                o.uv0 = UVAnim(o.uv0, _USpeed, _VSpeed);
                o.uv1 = UVAnim(o.uv1, _USpeed1, _VSpeed1);

                o.color = v.color * _Color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv0);
                half4 addCol  = SAMPLE_TEXTURE2D(_AddTex, sampler_AddTex, i.uv1);

                half4 mixed = mainCol * addCol * (mainCol + addCol);
                half4 col = i.color * mixed * _Intensity;

                return col;
            }

            ENDHLSL
        }
    }
}
