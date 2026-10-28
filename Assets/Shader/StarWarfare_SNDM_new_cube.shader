Shader "StarWarfare/SNDM_new_cube" {
	Properties{
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
		_Cube("Reflection Cubemap", CUBE) = "" {}
		_CubeColor("Reflection Color", Color) = (0,0,0,1)
	}

		SubShader{
			LOD 400
			Tags { "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf SNDMLight fullforwardshadows
			#pragma target 3.0
			#pragma multi_compile_fog

			sampler2D _RGBA;
			sampler2D _lightTex;
			sampler2D _normalmap;

			fixed4 _specColor;
			fixed  _lightRate;
			fixed  _specMultiple;
			fixed  _shininess;
			fixed4 _paintColor;
			fixed  _useCustomColor;
			samplerCUBE _Cube;
			fixed4 _CubeColor;

			struct Input {
				float2 uv_RGBA;
				float3 worldRefl;
				INTERNAL_DATA
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// Base albedo
				fixed4 baseTex = tex2D(_RGBA, IN.uv_RGBA);
				fixed4 lightTex = tex2D(_lightTex, IN.uv_RGBA);
				fixed4 normTex = tex2D(_normalmap, IN.uv_RGBA);

				// Light texture add
				fixed4 lightAdd = lightTex * _lightRate;

				// Paint color mask from normal map alpha
				fixed mask = normTex.a * _useCustomColor;
				fixed4 combined = lightAdd + (baseTex * (1.0 - mask)) + (_paintColor * mask);

				// Cubemap reflection, modulated by light texture alpha and cube color
				fixed4 refl = texCUBE(_Cube, IN.worldRefl);
				fixed3 reflContrib = (refl.rgb * lightTex.a) * _CubeColor.rgb;

				// Normal decode (tangent space)
				fixed3 n = UnpackNormal(normTex);

				o.Albedo = combined.rgb + reflContrib;
				o.Normal = n;
				o.Specular = 1.0;
				o.Gloss = _shininess;
				o.Alpha = 1.0;
			}

			inline fixed4 LightingSNDMLight(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten) {
				fixed3 h = normalize(lightDir + viewDir);
				fixed3 n = normalize(s.Normal);
				fixed3 l = normalize(lightDir);

				fixed ndl = max(0.0, dot(l, n));
				fixed nh = max(0.0, dot(n, h));

				// Luminance-weighted specular
				fixed lum = dot(_LightColor0.rgb, fixed3(0.22, 0.707, 0.071));

				fixed4 specCol = _specColor * _specMultiple;
				fixed spec = pow(nh, _shininess * 128.0) * lum;

				fixed3 lighting = _LightColor0.rgb * ndl;
				// Original: (albedo * light + light * (spec * specColor)) * 2
				fixed3 c = (s.Albedo * lighting + lighting * (spec * specCol.rgb)) * 2.0;
				// Also apply SH ambient — surface shaders do this automatically.
				return fixed4(c, s.Alpha);
			}
			ENDCG
		}

			Fallback "StarWarfare/Middle/SNDM_new"
}