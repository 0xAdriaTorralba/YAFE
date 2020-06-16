// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "FractalShaders/MandelbrotShaderUnlit" {
Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Zoom("Zoom", Vector) =  (1, 1, 1, 1)
        _Pan ("Pan", Vector) = (1, 1, 1, 1)
        _Aspect("Aspect Ratio", Float) = 1
        _Iterations ("Iterations", Range(1, 200000)) = 200
        _Threshold ("Threshold", Range(2, 250)) = 2
        _Colormap ("Colormap", Int) = 1
        _Algorithm ("Algorithm", Int) = 1
        _Degree ("Degree", Int) = 2
    }

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Zoom;
            float4 _Pan;
            int _Iterations;
            float _Aspect;
            float _Threshold;
            int _Colormap;
            int _Degree;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);
                float2 c = (i.texcoord - 0.5) * _Zoom.xy * float2(1, _Aspect) - _Pan.xy;
            //float2 c = (IN.uv_MainTex - 0.5) * 4.0f * float2(1, _Aspect) -float2(0.5f, 0.0f);//- _Pan.xy;
            //float2 c = (IN.uv_MainTex - 0.5) * 4.0f * float2(1, _Aspect) -float2(0.5f, 0.0f);
            float2 v = 0;
            float m = 0;
            const float r = _Threshold;

            for (int n = 0; n < _Iterations; ++n){
                // z_n = z_(n-1)^2 + c;
                if (_Degree == 2){
                    v = float2(
                        v.x * v.x - v.y * v.y, 
                        v.x * v.y * 2)
                    + c;
                }
                if (_Degree == 3){
                    v = float2(
                        v.x * v.x * v.x - 3 * v.x * v.y * v.y, 
                        3 * v.x * v.x * v.y - v.y * v.y * v.y
                    )
                    + c;
                }
                if (_Degree == 4){
                    v = float2(
                        v.x * v.x * v.x * v.x - 6 * v.x * v.x * v.y * v.y + v.y * v.y * v.y * v.y, 
                        4 * v.x * v.x * v.x * v.y - 4 * v.x * v.y * v.y * v.y
                    )
                    + c;
                }
                if (_Degree == 5){
                    v = float2(
                        pow(v.x, 5) - 10 * pow(v.x, 3) * pow(v.y, 2) + 5 * v.x * pow(v.y, 4),
                        5 * pow(v.x, 4) * v.y - 10 * pow(v.x, 2) * pow(v.y, 3) + pow(v.y, 5)
                    )
                    + c;
                }

                if (sqrt(dot(v, v)) < r){
                    m++;
                }
                v = clamp(v, -r, r);
                if (length(v) > _Threshold){
                    break;
                }
            }

            float4 color;
            if (_Colormap == 1){
                if (m == _Iterations){
                    color = float4(0, 0, 0, 1);
                }else{
                    color = float4(sin(m/4), sin(m/5), sin(m/7), 1) / 4 + 0.75;
                }
            }
            if (_Colormap == 2){
                if (m == _Iterations){
                    color = float4(0, 0, 0, 1);
                }else{
                    color = float4(cos(m/4), cos(m/5), cos(m/7), 1) / 4 + 0.75;
                }
            }     
            if (_Colormap == 3){
                if (m == _Iterations){
                    color = float4(0, 0, 0, 1);
                }else{
                    color = float4(tan(m/4), tan(m/5), tan(m/7), 1) / 4 + 0.75;
                }
            } 
            //color = normalize(color);

            return color;

            }
        ENDCG
    }
}

}
