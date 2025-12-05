Shader "Unlit/SpecularBamp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DiffuseColor ("Diffuse",Color) = (1,1,1,1)
        _SpecularColor ("Specular",Color) = (1,1,1,1)
        _gloss ("Gloss" ,Range(0,256)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="UniversalForward" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normalWS : TEXCOORD1;                                       //输出世界空间下法线信息
                float3 viewDirWS : TEXCOORD2;                                      //输出视角方向
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _DiffuseColor;
                float _gloss;
                float4 _SpecularColor;
            CBUFFER_END


            TEXTURE2D (_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                //法线从模型空间转世界空间
                o.normalWS = TransformObjectToWorldNormal(v.normalOS.xyz,true);
                //相机的世界坐标 减去 模型的世界坐标 得到 相机的视角方向
                o.viewDirWS = normalize(_WorldSpaceCameraPos.xyz - TransformObjectToWorld(v.positionOS.xyz));
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {


                Light mylight = GetMainLight();                                //获取场景主光源
                float4 LightColor = float4(mylight.color,1);                     //获取主光源的颜色

                float3 ViewDir = normalize(i.viewDirWS);                       //归一化视角方向
                float3 NormalDir = normalize(i.normalWS);
                float3 LightDir = normalize(mylight.direction);                //获取光照方向

                float3 halfDir = normalize(ViewDir + LightDir);                //半角向量，用于Blinn-Phong 视角方向 + 光照方向

                //float3 ReflectDir = normalize(reflect(-LightDir,NormalDir));     //反射方向 ,用于 Phong

                float LdotN = dot(NormalDir,LightDir) * 0.5 + 0.5;              //LdotN，半兰伯特
                half4 DiffuseTex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,float2(LdotN,LdotN));     //贴图采样变成3个变量

                //half4 specularValue = pow(max(0,dot(ReflectDir,ViewDir)),_gloss) * LightColor *_SpecularColor;   //计算高光  用于Phong
                half4 specularValue = pow(max(0,dot(NormalDir,halfDir)),_gloss) * LightColor *_SpecularColor;     //计算高光 用于Blinn-Phong
                float4 ambient = float4 (unity_AmbientSky.rgb,1);//环境光
                half4 diffusecolor = DiffuseTex * LdotN * LightColor * _DiffuseColor;//计算颜色
                half4 col =  specularValue + diffusecolor + ambient;

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}