Shader "Custom/UIAllFeatherColor"
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
                // 텍스처 색상
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 각 방향에서 가장 가까운 테두리까지의 거리 계산
                float distX = min(i.uv.x, 1.0 - i.uv.x);
                float distY = min(i.uv.y, 1.0 - i.uv.y);

                // 가장 짧은 거리(모서리 부분도 자연스럽게 흐려짐)
                float dist = min(distX, distY);

                // 페더 적용 (테두리 쪽일수록 알파 0)
                float featherAlpha = saturate(dist / _Feather);

                // 최종 색상 계산
                fixed4 finalColor = texColor * _Color;
                finalColor.a *= featherAlpha;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "UI/Unlit/Transparent"
}
