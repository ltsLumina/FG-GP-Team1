Shader "Custom/SimpleTextureArrayShader" {
    Properties {
        _MainTexArray ("Texture Array", 2DArray) = "" {}
        _ArrayIndex ("Array Index", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
            float _ArrayIndex;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample the texture array at the specified array index
                fixed4 color = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(i.uv, _ArrayIndex));
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}