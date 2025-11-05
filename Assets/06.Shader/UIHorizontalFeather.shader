Shader "Custom/UIHorizontalFeatherColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Feather ("Feather Width", Range(0.0, 0.9)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Feather;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 원본 텍스처 샘플링
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 가운데에서의 거리 계산 (UV.x 기준)
                float dist = abs(i.uv.x - 0.5);

                // 페더 계산 (양옆만 흐려지게)
                float featherAlpha = saturate((0.5 - dist) / _Feather);

                // 색상 적용 (Unity Image의 Color처럼)
                fixed4 finalColor = texColor * _Color;

                // 페더 알파 반영
                finalColor.a *= featherAlpha;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "UI/Unlit/Transparent"
}
