Shader "StarWarfare/SNDM_new_cube_test" {
	Properties{
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
		_seXiang("se xiang", Range(0,1)) = 0
		_seXiang_vector1("...", Vector) = (0,0,0,0)
		_seXiang_vector2("...", Vector) = (0,0,0,0)
		_Cube("Reflection Cubemap", CUBE) = "" {}
		_CubeColor("Reflection Color", Color) = (0,0,0,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 400

			CGPROGRAM
			#pragma surface surf CustomSNDM fullforwardshadows
			#pragma target 3.0

			sampler2D _RGBA, _lightTex, _normalmap;
			samplerCUBE _Cube;
			float4 _specColor, _paintColor, _CubeColor;
			float _lightRate, _specMultiple, _shininess, _useCustomColor, _seXiang;
			float4 _seXiang_vector1, _seXiang_vector2;

			struct Input {
				float2 uv_RGBA;
				float3 worldRefl;
				INTERNAL_DATA
			};

			void surf(Input IN, inout SurfaceOutput o) {
				float4 texBase = tex2D(_RGBA, IN.uv_RGBA);
				float4 texLight = tex2D(_lightTex, IN.uv_RGBA);
				float4 texNormal = tex2D(_normalmap, IN.uv_RGBA);

				float mx = max(max(texBase.r, texBase.g), texBase.b);
				float mn = min(min(texBase.r, texBase.g), texBase.b);
				float sat = (mx - mn) / (mx + 1e-05);
				float invSat = 1.0 - sat;
				float invSatZ = 1.0 - (sat*_seXiang_vector2.z);
				float invSatW = 1.0 - (sat*_seXiang_vector2.w);

				float3 seXiangColor;
				seXiangColor.x = dot(_seXiang_vector2.xy, float2(1, 1)) + dot(_seXiang_vector1, float4(invSatW, invSat, invSat, invSatZ));
				seXiangColor.y = dot(_seXiang_vector2.xy, float2(invSat, invSatW)) + dot(_seXiang_vector1, float4(invSat, invSatZ, 1, 1));
				seXiangColor.z = dot(_seXiang_vector2.xy, float2(invSatZ, invSat)) + dot(_seXiang_vector1, float4(1, 1, invSatW, invSat));

				float4 colorTint;
				colorTint.w = 1.0;
				colorTint.xyz = seXiangColor * mx;

				float factor = texNormal.w * _useCustomColor;
				float4 combined = texLight * _lightRate
					+ colorTint * factor
					+ texBase * (1.0 - factor);

				o.Normal = UnpackNormal(texNormal);

				float3 worldRefl = WorldReflectionVector(IN, o.Normal);

				float4 refl = texCUBE(_Cube, worldRefl);

				o.Albedo = combined.rgb + (refl.rgb * texLight.w * _CubeColor.rgb);

				o.Specular = _specColor.rgb * _specMultiple;
				o.Gloss = _shininess;
				o.Alpha = 1.0;
			}

			inline fixed4 LightingCustomSNDM(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half3 h = normalize(lightDir + viewDir);
				half diff = max(0, dot(s.Normal, lightDir));
				half nh = max(0, dot(s.Normal, h));
				half specPow = pow(nh, s.Gloss * 128.0);
				half specLum = dot(_LightColor0.rgb, half3(0.22, 0.707, 0.071));
				half3 spec = s.Specular * specPow * specLum;
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + spec * _LightColor0.rgb) * 2.0 * atten;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}
			Fallback "StarWarfare/Middle/SNDM_new"
}