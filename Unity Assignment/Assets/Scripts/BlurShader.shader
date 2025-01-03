Shader "Hidden/BlurShader"
{

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // UI 텍스처
        _BlurSize ("Blur Size", Range(0, 10)) = 2.0 // 블러 강도
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 blurOffset = _BlurSize / _ScreenParams.xy; // 화면 크기 기준 블러 오프셋
                fixed4 color = 0;

                // 블러 샘플링 (8 방향)
                color += tex2D(_MainTex, uv + blurOffset * float2(-1, -1));
                color += tex2D(_MainTex, uv + blurOffset * float2(-1,  1));
                color += tex2D(_MainTex, uv + blurOffset * float2( 1, -1));
                color += tex2D(_MainTex, uv + blurOffset * float2( 1,  1));
                color += tex2D(_MainTex, uv + blurOffset * float2(-1,  0));
                color += tex2D(_MainTex, uv + blurOffset * float2( 1,  0));
                color += tex2D(_MainTex, uv + blurOffset * float2( 0, -1));
                color += tex2D(_MainTex, uv + blurOffset * float2( 0,  1));

                // 평균화
                color /= 8;

                return color;
            }
            ENDCG
        }
    }
}