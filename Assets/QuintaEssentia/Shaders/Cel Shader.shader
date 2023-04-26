Shader "Unlit/Cel Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.4, 0.4, 0.4, 1.0)
        _Color("Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.02
        _RimSize("Rim size", Range(-1, 1)) = 0.5
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
        _Glossiness("Glossiness", Float) = 32

        [HDR][PerRendererData]
        _SpecularColor("Specular Color", Color) = (0.4, 0.4, 0.4, 1.0)
        [HDR][PerRendererData]
        _RimColor("Rim Color", Color) = (0.4, 0.4, 0.4, 1.0)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                Cull off

                Stencil
                {
                    Ref 1
                    Comp Always
                    Pass Replace
                }

                Tags
                {
                    "LightMode" = "ForwardBase"
                    "PassFlags" = "OnlyDirectional"
                }

                CGPROGRAM
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile_fwdbase

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "AutoLight.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldNormal : NORMAL;
                    float3 viewDir : TEXCOORD1;

                    SHADOW_COORDS(2)
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _AmbientColor)
                    UNITY_DEFINE_INSTANCED_PROP(float, _RimSize)
                    UNITY_DEFINE_INSTANCED_PROP(float, _RimThreshold)
                    UNITY_DEFINE_INSTANCED_PROP(float, _Glossiness)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _RimColor)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _SpecularColor)
                UNITY_INSTANCING_BUFFER_END(Props)

                v2f vert(appdata v)
                {
                    UNITY_SETUP_INSTANCE_ID(v);

                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.viewDir = WorldSpaceViewDir(v.vertex);

                    UNITY_TRANSFER_INSTANCE_ID(v, o);

                    TRANSFER_SHADOW(o)
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(i);

                    fixed4 color = tex2D(_MainTex, i.uv);

                    float3 viewDir = normalize(i.viewDir);

                    float3 normal = normalize(i.worldNormal);
                    float NdotL = dot(_WorldSpaceLightPos0, normal);
                    float shadow = SHADOW_ATTENUATION(i);
                    float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
                    float4 light = lightIntensity * _LightColor0;

                    float4 rimDot = 1 - dot(viewDir, normal);
                    float rimIntensity = rimDot * pow(NdotL, UNITY_ACCESS_INSTANCED_PROP(Props, _RimThreshold));
                    rimIntensity = smoothstep(UNITY_ACCESS_INSTANCED_PROP(Props, _RimSize) - 0.01, UNITY_ACCESS_INSTANCED_PROP(Props, _RimSize) + 0.01, rimIntensity);
                    float4 rim = rimIntensity * UNITY_ACCESS_INSTANCED_PROP(Props, _RimColor);

                    float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
                    float NdotH = dot(normal, halfVector);
                    float specularIntensity = pow(NdotH * lightIntensity, UNITY_ACCESS_INSTANCED_PROP(Props, _Glossiness) * UNITY_ACCESS_INSTANCED_PROP(Props, _Glossiness));
                    float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
                    float4 specular = specularIntensitySmooth * UNITY_ACCESS_INSTANCED_PROP(Props, _SpecularColor);

                    return color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * (UNITY_ACCESS_INSTANCED_PROP(Props, _AmbientColor) + light) + specular + rim;
                }
                ENDCG
            }

            //OUTLINE//

            Pass
            {
                Cull Off

                Stencil
                {
                    Ref 1
                    Comp Greater
                }

                CGPROGRAM
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(half, _OutlineWidth)
                UNITY_INSTANCING_BUFFER_END(Props)

                static const half4 OUTLINE_COLOR = half4(0,0,0,0);

                v2f vert(appdata v)
                {
                    UNITY_SETUP_INSTANCE_ID(v);

                    v.vertex.xyz += v.normal * UNITY_ACCESS_INSTANCED_PROP(Props, _OutlineWidth);

                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    UNITY_TRANSFER_INSTANCE_ID(v, o);

                    return o;
                }

                fixed4 frag() : SV_Target
                {
                    return OUTLINE_COLOR;
                }
                ENDCG
            }

            UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
        }

            FallBack "Mobile/Diffuse"
}