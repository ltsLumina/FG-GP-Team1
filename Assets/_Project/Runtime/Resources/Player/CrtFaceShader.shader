Shader "Custom/CRTShaderWithArray" {
    Properties {
        _MainTexArray ("Texture Array", 2DArray) = "" {}
        _ArrayIndex ("Array Index", Range(0, 30)) = 0
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _DistortionAmount ("Distortion Amount", Range(0, 1)) = 0.1
        _AberrationAmount ("Aberration Amount", Range(0, 0.1)) = 0.03
    }
    SubShader {
        Tags { "Queue"="Transparent-10" "RenderType"="Transparent" }  // Fine-tuned queue order
        LOD 100

        Pass {
            ZWrite On                 // Enable depth writing
            ZTest LEqual              // Respect depth to avoid rendering too prominently
            Fog { Mode Off }          // Disable fog interactions

            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Declare texture array
            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
            float _ArrayIndex;
            float _ScanlineIntensity;
            float _DistortionAmount;
            float _AberrationAmount;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 ApplyAberration(float2 uv, float amount, int arrayIndex) {
                // Apply chromatic aberration by shifting color channels
                float3 color;
                color.r = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(uv + amount, arrayIndex)).r;
                color.g = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(uv, arrayIndex)).g;
                color.b = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(uv - amount, arrayIndex)).b;
                return color;
            }

            float3 ApplyScanlines(float3 color, float2 uv) {
                // Simulate scanlines
                float scanline = sin(uv.y * 800.0) * _ScanlineIntensity;
                return color * (1.0 - scanline);
            }

            float2 ApplyDistortion(float2 uv, float amount) {
                // Apply CRT distortion (curvature effect)
                uv = uv * 2.0 - 1.0;
                uv *= 1.0 + amount * (dot(uv, uv));
                return uv * 0.5 + 0.5;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Convert _ArrayIndex to an integer to prevent interpolation issues
                int arrayIndex = round(_ArrayIndex);  // Ensure the index is an integer

                // Apply distortion, aberration, and scanlines
                float2 distortedUV = ApplyDistortion(i.uv, _DistortionAmount);
                float3 color = ApplyAberration(distortedUV, _AberrationAmount, arrayIndex);
                color = ApplyScanlines(color, i.uv);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
