// Modify based on Unity Shader
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// References
// https://github.com/Chlumsky/msdfgen
// https://dribbble.com/shots/10084381-Neomorphism-Guide-2-0-Original

Shader "UI/Neumorphism"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.56, 0.62, 0.68, 1)
        _PixelRange ("Pixel Range", Range(0, 20)) = 4
        _BevelSize ("Bevel Size", Range(0, 1)) = 0.01
        _Height ("Height", Range(-10, 10)) = 4
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "UnityUI.cginc"
        #include "UnityLightingCommon.cginc"
        #include "./msdf.cginc"

        #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
        #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1; // Gradient
            float2 texcoord2 : TEXCOORD2; // Shadow
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color    : COLOR;
            float2 texcoord  : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
            float4 worldPosition : TEXCOORD3;
            float3 lightDir : TEXCOORD4;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        fixed4 _Color;
        fixed4 _LightColor;
        fixed4 _ShadowColor;
        float _PixelRange;
        float _BevelSize;
        float _Height;
        
        fixed4 _TextureSampleAdd;
        float4 _ClipRect;
        float4 _MainTex_ST;

        float4x4 _WorldRotation;

        v2f vert(appdata_t v)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            OUT.lightDir = normalize(_WorldSpaceLightPos0.xyz - v.vertex * _WorldSpaceLightPos0.w);
            OUT.lightDir = mul(_WorldRotation, OUT.lightDir);

            v.vertex.xy += OUT.lightDir * -v.texcoord2.x;

            OUT.worldPosition = v.vertex;
            OUT.vertex = UnityObjectToClipPos(v.vertex);

            OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            OUT.texcoord1 = v.texcoord1;
            OUT.texcoord2 = v.texcoord2;

            OUT.color = v.color * _Color;
            return OUT;
        }

        inline half4 neu_main(v2f IN)
        {
            half3 sample = tex2D(_MainTex, IN.texcoord);

            float2 sdfUnit = _PixelRange / _MainTex_TexelSize.zw;
            float sdf = msdf(_MainTex, IN.texcoord);
            float clipSdf = sdf * max(dot(sdfUnit, 0.5 / fwidth(IN.texcoord)), 1);
            
            half4 color = IN.color;
            color.a *= saturate(clipSdf + 0.5);

            // Inner shadow
            if(_Height < 0)
            {
                float3 normal = msdf_normal(_MainTex, _MainTex_TexelSize, IN.texcoord);
                float bevel = msdf_light(IN.lightDir, normal * _Height);
                color.rgb = lerp(color.rgb, _LightColor, smoothstep(0, 1, saturate(bevel)));
                color.rgb = lerp(color.rgb, _ShadowColor.rgb, smoothstep(0, 1, saturate(-bevel)));
            }

            // Bevel Out Line
            if(sdf < _BevelSize)
            {
                float3 normal = msdf_normal(_MainTex, _MainTex_TexelSize, IN.texcoord);
                float bevel = msdf_light(IN.lightDir, normal * _Height);
                color.rgb = lerp(color.rgb, _LightColor, saturate(bevel));
                color.rgb = lerp(color.rgb, _ShadowColor.rgb, saturate(-bevel));
            }

            // Gradient
            color.rgb = lerp(color.rgb, _ShadowColor.rgb, (1.0 - msdf_light(IN.lightDir,  float3(IN.texcoord1, 0))) * 0.2);
            return color;
        }

        inline half4 neu_shadow(v2f IN)
        {
            if(_Height < 0)
            {
                return half4(1, 1, 1, 0);
            }
            half3 sample = tex2D(_MainTex, IN.texcoord);

            float2 sdfUnit = _PixelRange / _MainTex_TexelSize.zw;
            float sdf = msdf(_MainTex, IN.texcoord);
            float clipSdf = sdf * max(dot(sdfUnit, 0.5 / fwidth(IN.texcoord)), 1);
            
            half4 color = IN.color;
            color.a *= saturate(sdf + 0.5) * (_Height / 5);

            return color;
        }


        fixed4 frag(v2f IN) : SV_Target
        {
            half4 color = length(IN.texcoord2) > 0 ? neu_shadow(IN) : neu_main(IN);
            
            color += _TextureSampleAdd;

            #ifdef UNITY_UI_CLIP_RECT
            color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif

            #ifdef UNITY_UI_ALPHACLIP
            clip (color.a - 0.001);
            #endif

            return color;
        }
        
        ENDCG

        Pass
        {
            Name "Main"
            Tags {  "LightMode"="ForwardBase" }
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask [_ColorMask]

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
        ENDCG
        }
    }
}
