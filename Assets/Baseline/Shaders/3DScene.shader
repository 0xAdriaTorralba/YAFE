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
			uniform int _IFSIters;

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
		float basic_box(float3 pos, float3 b){
    float3 d = abs(pos) - b;
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}
	


		float sdBox( float3 p, float3 b ) {
			float3 di = abs(p) - b;
			float mc = max(di.x,max(di.y,di.z));
			return min(mc,length(max(di,0.0)));
		}

		float MengerSponge2(float3 p){

			float main_width_b = 4.0;
			float inf = 50.0;

			float hole_x, hole_y, hole_z;
			float hole_width_b = main_width_b / 3.0;
			
			float menger = basic_box(p, float3(main_width_b, main_width_b, main_width_b));
			
			for (int iter=0; iter<_IFSIters; iter++){

				float hole_distance = hole_width_b * 6.0;
		
				float3 c = float3(hole_distance, hole_distance, hole_distance);
				float3 q = fmod(p + float3(hole_width_b, hole_width_b, hole_width_b), c) - float3(hole_width_b, hole_width_b, hole_width_b);

				hole_x = basic_box(q, float3(inf, hole_width_b, hole_width_b));
				hole_y = basic_box(q, float3(hole_width_b, inf, hole_width_b));
				hole_z = basic_box(q, float3(hole_width_b, hole_width_b, inf));

				hole_width_b = hole_width_b / 3.0;        // reduce hole size for next iter
				menger = max(max(max(menger, -hole_x), -hole_y), -hole_z); // subtract

			}

			return menger;

		}

		float MengerSponge( in float3 p ) {
			float d = basic_box(p,float3(10.0, 10.0, 10.0));
			float s = .05;
			for( int m=0; m<_IFSIters; m++ ) {
				float3 a = frac( p*s )-.5;
				s *= 3.;
				float3 r = abs(1.-6.*abs(a));
				float da = max(r.x,r.y);
				float db = max(r.y,r.z);
				float dc = max(r.z,r.x);
				float c = (min(da,min(db,dc))-1.0)/(2.*s);
				if( c>d ) {
					d = c;
				}
			}
			return d;
		}

		float calcSoftshadow( in float3 ro, in float3 rd, in float mint, in float tmax ) {
			float res = 1.0;
			float t = mint;
			float ph = 1e10; 
			for( int i=0; i<32; i++ ) {
				float h = MengerSponge( ro + rd*t );
				float y = h*h/(2.0*ph);
				float d = sqrt(max(0.,h*h-y*y));
				res = min( res, 8.0*d/max(0.0001,t-y) );
				ph = h;
				t += h;//min(h, .1);// clamp( h, 0.02, 0.10 );
				if( res<0.001 || t>tmax ) break;
			}
			return clamp( res, 0.0, 1.0 );
		}

		float calcAO( in float3 pos, in float3 nor ) {
			float occ = 0.0;
			float sca = 1.0;
			for( int i=0; i<5; i++ ) {
				float hr = 0.01 + 0.5*float(i)/4.0;
				float3 aopos =  nor * hr + pos;
				float dd = MengerSponge( aopos );
				occ += -(dd-hr)*sca;
				sca *= 0.9;
			}
			return clamp( 1. - 3.0*occ, 0.0, 1.0 );    
		}

		float3 calcNormal(in float3 pos) {
			float3  eps = float3(.001,0.0,0.0);
			float3 nor;
			nor.x = MengerSponge(pos+eps.xyy) - MengerSponge(pos-eps.xyy);
			nor.y = MengerSponge(pos+eps.yxy) - MengerSponge(pos-eps.yxy);
			nor.z = MengerSponge(pos+eps.yyx) - MengerSponge(pos-eps.yyx);
			return normalize(nor);
		}

		float4 tex3D( sampler2D sam, in float3 p, in float3 n ) {
			float4 x = tex2D( sam, p.yz );
			float4 y = tex2D( sam, p.zx );
			float4 z = tex2D( sam, p.xy );

			return x*abs(n.x) + y*abs(n.y) + z*abs(n.z);
		}




			float Sierpinski(float3 z)

			{				
				float Scale = 2.0f;
				int Iterations = _IFSIters;
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

				return length(z) * pow(Scale, float(-n));
			}



			float sphereDE(float3 p){
				return length(p) - 1.0f;
			}


			float mandelbulbDE(float3 pos){
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
				return 0.5f * log(r) * r/dr;
			}


			uniform int maxstep;
			float4 trace(float3 from, float3 direction, v2f f) {
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
						res = Sierpinski(t);
					}
					if (_type == 3){
						res = MengerSponge(t);
					}
					totalDistance += res.x;
					
					if (res.x < MinimumDistance) {
						break;
					}


				}
				
				aux1 = float3(t.x+h, t.y, t.z);
				aux2 = float3(t.x-h, t.y, t.z);
				if (_type == 1){
					normal_x = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2 || _type == 3){
					normal_x = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				aux1 = float3(t.x, t.y+h, t.z);
				aux2 = float3(t.x-h, t.y-h, t.z);
				if (_type == 1){
					normal_y = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2 || _type == 3){
					normal_y = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				aux1 = float3(t.x, t.y, t.z+h);
				aux2 = float3(t.x, t.y, t.z-h);
				if (_type == 1){
					normal_z = ( mandelbulbDE(aux1) - mandelbulbDE(aux2) ) / ( 2*h );
				}
				if (_type == 2 || _type == 3){
					normal_z = ( sphereDE(aux1) - sphereDE(aux2) ) / ( 2*h );
				}

				normal = float3(normal_x, normal_y, normal_z);
				normal = normalize(normal);

				if (_type == 3){
					float3 col;
					if (i == maxstep){ return float4(0.0, 0.0, 0.0, 1.0);}

					if (totalDistance < 100) {
						float3 p = from + totalDistance * direction;
						float3 n = calcNormal(p);
						float3 ref = reflect(direction, n);

						float ao = .4 + .6 * calcAO(p, n);
						float sh = .4 + .6 * calcSoftshadow(p, _LightDir, 0.005, 1.);
					
						float diff = max(0.,dot(_LightDir,n)) * ao * sh;
						float amb  = (.4+.2*n.y) * ao * sh;
						float spe = pow(clamp(dot(ref,_LightDir), 0., 1.),8.) * sh * .5;
						
						float3 mat = tex3D(_MainTex, p, n).rgb;
						col = (amb + diff) * lerp(float3(.4,.6,.8),float3(.1,.2,.3),mat.r) + spe * dot(mat,mat);
						//col *= float3(172/255.0, 167/255.0, 176/255.0);
						//col *= normal;
				}
				
					// gamma
					col = lerp(col, sqrt( clamp(col,float3(0,0, 0),float3(1, 1, 1 ))), .99);
					//col = normalize(col);
					return float4(clamp(col,float3(0, 0, 0),float3(2, 2, 2)), 1.0);
				}

				if (i == maxstep){ 
					return float4(0.0, 0.0, 0.0, 1.0);
				}else{
					color = normalize(color);
					color = float4(
							1 - (float(i)/float(maxstep))+normal_x, 
							1 - (float(i)/float(maxstep))+normal_y, 
							1 - (float(i)/float(maxstep))+normal_z, 
							1.0
					);
					color = float4(pow(color.x, 6.2), pow(color.y, 6.2), pow(color.z, 6.2), 1.0);
					return color;
				}
				
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

				fixed4 add = trace(ro, rd, i);



				// Returns final color using alpha blending
				return fixed4(col*(1.0 - add.w) + add.xyz * add.w,1.0);
			}
			ENDCG
		}
	}
}