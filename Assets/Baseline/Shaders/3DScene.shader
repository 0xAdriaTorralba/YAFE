// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fractals/3DScene"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Compile one version of the shader with performance debugging
			// This way we can simply set a shader keyword to test perf
			#pragma multi_compile __ DEBUG_PERFORMANCE
			// You may need to use an even later shader model target, depending on how many instructions you have
			// or if you need variable-length for loops.
			#pragma target 3.0

			#include "UnityCG.cginc"
			
			uniform sampler2D _CameraDepthTexture;
			// These are are set by our script (see RaymarchGeneric.cs)
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;

			uniform float4x4 _CameraInvViewMatrix;
			uniform float4x4 _FrustumCornersES;
			uniform float4 _CameraWS;

			uniform float3 _LightDir;
			uniform float4x4 _MatTorus_InvModel;
			uniform sampler2D _ColorRamp_Material;
			uniform sampler2D _ColorRamp_PerfMap;
			uniform float _Power;
			uniform int _type;

			uniform int _DrawDistance;

			struct appdata
			{
				// Remember, the z value here contains the index of _FrustumCornersES to use
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				
				// Index passed via custom blit function in RaymarchGeneric.cs
				half index = v.vertex.z;
				v.vertex.z = 0.1;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				// Get the eyespace view ray (normalized)
				o.ray = _FrustumCornersES[(int)index].xyz;
				// Dividing by z "normalizes" it in the z axis
				// Therefore multiplying the ray by some number i gives the viewspace position
				// of the point on the ray with [viewspace z]=i
				o.ray /= abs(o.ray.z);

				// Transform the ray from eyespace to worldspace
				o.ray = mul(_CameraInvViewMatrix, o.ray);

				return o;
			}

			// int last = 0;
			// float escapeLength(in float3 pos)
			// {
			// 	float3 z = pos;
			// 	for(int i=1; i < 10; i++)
			// 	{
			// 			last = i;
			// 			//return length(z);
			// 	}	
			// 	return length(z);
			// }

			// float mandelbrotDE(float3 p) {
			// 	last = 0;
			// 	float r = escapeLength(p);
			// 	if (r*r<5) return 0.0;
			// 	float3 gradient = (float3(escapeLength(p+xDir*EPS), escapeLength(p+yDir*EPS), escapeLength(p+zDir*EPS))-r)/EPS;
			// 	return 0.5*r*log(r)/length(gradient);
			// }

			float2 DE(float3 z)

			{				
				float Scale = 2.0f;
				int Iterations = 15;
				float3 a1 = float3(1,1,1);
				float3 a2 = float3(-1,-1,1);
				float3 a3 = float3(1,-1,-1);
				float3 a4 = float3(-1,1,-1);
				float3 c;
				int n = 0;
				float dist, d;
				while (n < Iterations) {
					c = a1; 
					dist = length(z-a1);
					d = length(z-a2); 
					if (d < dist) { c = a2; dist=d; }
					d = length(z-a3); 
					if (d < dist) { c = a3; dist=d; }
					d = length(z-a4); 
					if (d < dist) { c = a4; dist=d; }
					z = Scale*z - c*(Scale-1.0);
					n++;
				}

				return float2(length(z) * pow(Scale, float(-n)), n);
			}



			float sphereDE(float3 p){
				return length(p) - 1.0f;
			}


			float2 mandelbulbDE(float3 pos){
				float3 z = pos;
				float dr = 1.0f;
				float r = 0.0f;
				int maxIters = 64;
				int i;
				for (i = 0; i < maxIters; i++){
					r = length(z);
					if (r > 5){
						break;
					}
					// convert to polar coordinates
					float theta = acos(z.z / r);
					float phi = atan(z.y / z.x);
					dr = pow(r, _Power - 1.0) * _Power * dr + 1.0f;

					// scale and rotate the point
					float zr = pow(r, _Power);
					theta = theta * _Power;
					phi = phi * _Power;

					// convert back to cartesian coordinates
					z  = zr * float3(sin(theta) * cos(phi), 
									 sin(phi) * sin(theta), 
									 cos(theta));
					z += pos;
				}
				return float2(0.5f * log(r) * r/dr, i);
			}


			uniform int maxstep;
			float4 trace(float3 from, float3 direction) {
				float totalDistance = 0.0f;
				float h = 0.25;
				float3 aux1, aux2;
				float normal_x, normal_y, normal_z;
				float3 normal;
				float4 color;
				float MinimumDistance = length(from - direction)/_DrawDistance;
				float3 t = 0;
				float light;
				float n;
				int i;
				float2 res;
				for (i = 0; i < maxstep; i++) {
					t = from + totalDistance * direction;
					if (_type == 1){
						res = mandelbulbDE(t); // res.x = distance, res.y = iter on el fractal 'convergeix'
					}
					if (_type == 2){
						res = DE(t);
					}
					if (_type == 3){
						res = sphereDE(t);
					}
					totalDistance += res.x;
					
					//if (res.y == 64) break; // 64 \equiv maxiters de mandelbulbDE de NO convergir
					if (res.x < MinimumDistance || res.y > maxstep) {
						break;
					}

					// if (res.y == 64){
					// 	color = float4(0, 0, 0, 1);
					// }else{
					// 	color = float4(sin(res.y/4), sin(res.y/5), sin(res.y/7), 1) / 4 + 0.75;
         			// }	

				}
				//return color;
				// TODO posar color com a 2D (usant el iterat)
				
				aux1 = float3(t.x+h, t.y, t.z);
				aux2 = float3(t.x-h, t.y, t.z);
				if (_type == 1){
					normal_x = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2){
					normal_x = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				aux1 = float3(t.x, t.y+h, t.z);
				aux2 = float3(t.x-h, t.y-h, t.z);
				if (_type == 1){
					normal_y = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2){
					normal_y = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				aux1 = float3(t.x, t.y, t.z+h);
				aux2 = float3(t.x, t.y, t.z-h);
				if (_type == 1){
					normal_z = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2){
					normal_z = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				normal = float3(normal_x, normal_y, normal_z);

				normal = normalize(normal);

				//return 1.0 - float(i) /float(maxstep);
				//return float4(1.0-float(i)/float(maxstep), 1.0-float(i)/float(maxstep), 1.0-float(i)/float(maxstep), 1.0);
				if (i/float(maxstep) > 0.8){ return float4(0.0, 0.0, 0.0, 1.0);}
				color = float4(
							1 - (float(i)/float(maxstep))+normal_x, 
							1 - (float(i)/float(maxstep))+normal_y, 
							1 - (float(i)/float(maxstep))+normal_z, 
							1.0
						);
				//color = float4(1.0, 1.0, 1.0, 0.0) + color;
				//color = color * (1.0 / 2.0);
				color.w = 1.0;
				color = float4(pow(color.x, 6.2), pow(color.y, 6.2), pow(color.z, 6.2), 1.0);
				//color = normalize(color);
				return color;
				//return fixed4(tex2D(_ColorRamp_Material, float2(t.y,0)).xyz * light, 1);
			}


			fixed4 frag (v2f i) : SV_Target
			{
				// ray direction
				float3 rd = normalize(i.ray.xyz);
				// ray origin (camera position)
				// Observador
				float3 ro = _CameraWS;
				float4 color;

				float2 duv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					duv.y = 1 - duv.y;
				#endif

				// Convert from depth buffer (eye space) to true distance from camera
				// This is done by multiplying the eyespace depth by the length of the "z-normalized"
				// ray (see vert()).  Think of similar triangles: the view-space z-distance between a point
				// and the camera is proportional to the absolute distance.
				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, duv).r);
				depth *= length(i.ray);

				fixed3 col = tex2D(_MainTex,i.uv);

				fixed4 add = trace(ro, rd);


				// Returns final color using alpha blending
				return fixed4(col*(1.0 - add.w) + add.xyz * add.w,1.0);
			}
			ENDCG
		}
	}
}