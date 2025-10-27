Shader "Custom/UI_BillboardForCanvas"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _LockY("Lock Y Axis", Float) = 1 // 1이면 Y축 고정
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _LockY;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 col : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;

                    // --- 오브젝트 중심의 월드 좌표 ---
                    float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                    // --- 카메라 방향 계산 ---
                    float3 camPos = _WorldSpaceCameraPos;
                    float3 forward = normalize(camPos - worldCenter);

                    // Y축 고정 옵션
                    if (_LockY > 0.5)
                    {
                        forward.y = 0;
                        forward = normalize(forward);
                    }

                    float3 up = float3(0,1,0);
                    float3 right = normalize(cross(up, forward));
                    up = cross(forward, right);

                    // --- 로컬 좌표를 카메라 기준 회전으로 변환 ---
                    float3 worldPos = worldCenter + v.vertex.x * right + v.vertex.y * up;

                    // --- 클립 공간으로 변환 ---
                    o.pos = UnityWorldToClipPos(worldPos);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.col = _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 texColor = tex2D(_MainTex, i.uv);
                    return texColor * i.col;
                }

                ENDCG
            }
        }
}
