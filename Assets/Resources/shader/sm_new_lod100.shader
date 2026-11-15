Shader "StarWarfare/Middle/SNDM_new_LOD100" {
	Properties{
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_normalmap("Normalmap", 2D) = "bump" {}
		_CubeColor("Reflection Color", Color) = (0,0,0,1)
	}

		SubShader{
			LOD 100
			Tags { "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf SNDMLight fullforwardshadows
			#pragma target 3.0
			#pragma multi_compile_fog

			sampler2D _RGBA;
			sampler2D _lightTex;
			sampler2D _normalmap;

			fixed  _lightRate;
			fixed  _specMultiple;
			samplerCUBE _Cube;
			fixed4 _CubeColor;

			struct Input {
				float2 uv_RGBA;
				float3 worldRefl;
				INTERNAL_DATA
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 baseTex = tex2D(_RGBA, IN.uv_RGBA);
				fixed4 lightTex = tex2D(_lightTex, IN.uv_RGBA);
				fixed4 normTex = tex2D(_normalmap, IN.uv_RGBA);

				fixed4 combined = (lightTex * _lightRate) + baseTex;

				fixed4 refl = texCUBE(_Cube, IN.worldRefl);
				fixed3 reflContrib = (refl.rgb * lightTex.a) * _CubeColor.rgb;

				fixed3 n = UnpackNormal(normTex);

				o.Albedo = combined.rgb + reflContrib;
				o.Normal = n;
				o.Specular = 0.0;   
				o.Gloss = 0.0;      
				o.Alpha = 1.0;
			}

			inline fixed4 LightingSNDMLight(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten) {
				fixed3 n = normalize(s.Normal);
				fixed3 l = normalize(lightDir);

				fixed ndl = max(0.0, dot(l, n));

				fixed3 lighting = _LightColor0.rgb * ndl;

				fixed3 c = (s.Albedo * lighting) * 2.0;
				return fixed4(c, s.Alpha);
			}
			ENDCG
		}

			Fallback "Mobile/Diffuse"
}