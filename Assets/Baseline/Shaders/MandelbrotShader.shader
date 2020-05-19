Shader "FractalShaders/MandelbrotShader"
{
    Properties
    {
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
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };


        float4 _Zoom;
        float4 _Pan;
        int _Iterations;
        float _Aspect;
        float _Threshold;
        int _Colormap;
        int _Degree;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            float2 c = (IN.uv_MainTex - 0.5) * _Zoom.xy * float2(1, _Aspect) - _Pan.xy;
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
                if (_Degree == 6){
                    v = float2(
                        pow(v.x, 6) - 15 * pow(v.x, 4) * pow(v.y, 2) + 15 * pow(v.x, 2) * pow(v.y, 4) - pow(v.y, 6),
                        6 * pow(v.x, 5) * v.y - 20 * pow(v.x, 3) * pow(v.y, 3) + 6 * v.x * pow(v.y, 5)
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
            o.Albedo = color;

            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
