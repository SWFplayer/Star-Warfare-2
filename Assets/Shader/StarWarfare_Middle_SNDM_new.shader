Shader "StarWarfare/Middle/SNDM_new" {
	Properties{
		_maincolor("Main Color", Color) = (1,1,1,1)
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_specTex_Gloss("Specular(RGB)Gloss(A)", 2D) = "black" {}
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
	}

		SubShader{
			LOD 300
			Tags { "RenderType" = "Opaque" "IgnoreProjector" = "False" }
			Fog { Mode Off }

			CGPROGRAM
			#pragma surface surf CustomSNDM fullforwardshadows
			#pragma target 3.0

			sampler2D _RGBA;
			sampler2D _lightTex;
			sampler2D _normalmap;
			sampler2D _specTex_Gloss;

			fixed4 _maincolor;
			fixed4 _paintColor;
			fixed4 _specColor;
			half _lightRate;
			half _useCustomColor;
			half _specMultiple;
			half _shininess;

			struct Input {
				float2 uv_RGBA;
				float2 uv_normalmap;
				float2 uv_specTex_Gloss;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// 1. Sample Textures
				fixed4 baseTex = tex2D(_RGBA, IN.uv_RGBA);
				fixed4 lightTex = tex2D(_lightTex, IN.uv_RGBA);
				fixed4 normalTex = tex2D(_normalmap, IN.uv_normalmap);
				fixed4 specTex = tex2D(_specTex_Gloss, IN.uv_specTex_Gloss);

				// 2. Calculate Albedo (matching the GLSL's paint mask logic)
				if (_useCustomColor > 0.5) {
					o.Albedo = (baseTex.rgb * (1.0 - normalTex.a))
							 + (lightTex.rgb * _lightRate)
							 + (_paintColor.rgb * normalTex.a);
				}
		 else {
		  o.Albedo = baseTex.rgb + (lightTex.rgb * _lightRate);
	  }

				// 3. Setup Normal
				o.Normal = UnpackNormal(normalTex);

				// 4. Setup Specular and Gloss
				o.Specular = specTex.rgb * _specMultiple;
				o.Gloss = _shininess;

				o.Alpha = 1.0;
			}

			// Custom Lighting Function
			inline fixed4 LightingCustomSNDM(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half3 h = normalize(lightDir + viewDir);
				half diff = max(0, dot(s.Normal, lightDir));

				half nh = max(0, dot(s.Normal, h));
				// The GLSL uses Gloss * 128 for the specular power
				half specPow = pow(nh, s.Gloss * 128.0);

				// The GLSL specular uses a luminance factor
				half specLuminance = dot(_LightColor0.rgb, half3(0.22, 0.707, 0.071));
				half3 spec = s.Specular * specPow * specLuminance;

				fixed4 c;
				// The GLSL multiplies the final result by 2.0
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + spec * _LightColor0.rgb) * 2.0 * atten;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}

			Fallback "StarWarfare/Middle/SNDM_new_LOD100"
}