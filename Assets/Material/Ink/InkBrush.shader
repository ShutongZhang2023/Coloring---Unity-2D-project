Shader "Custom/InkBrush"
{
   Properties {
        _Color ("Brush Color", Color) = (1, 0, 0, 0.5) // 可带透明度
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _Color; 
            }
            ENDCG
        }
    }
}

