Shader "StarWarfare/SNDM_new_outline" {
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
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.002,0.1)) = 0.006
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" "IgnoreProjector" = "False" }
			LOD 500

			Pass {
				Name "OUTLINE"
				Tags { "LightMode" = "Always" }

				ZTest Always
				ZWrite Off
				Cull Front
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask RGB

				Stencil {
					Ref 2
					Comp NotEqual
					ZFail Zero
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
				};

				float _Outline;
				fixed4 _OutlineColor;

				v2f vert(appdata v) {
					v2f o;
					
					float4 pos = v.vertex;
					pos.xyz += normalize(v.normal) * _Outline;
					o.pos = UnityObjectToClipPos(pos);
					o.color = _OutlineColor;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					return i.color;
				}
				ENDCG
			}

			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }

				Stencil {
					Ref 2
					Pass Replace
					ZFail Replace
				}

					
					ColorMask 0
					ZWrite Off
				}

					CGPROGRAM
					#pragma surface surf CustomLight fullforwardshadows
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
						fixed4 baseTex = tex2D(_RGBA, IN.uv_RGBA);
						fixed4 lightTex = tex2D(_lightTex, IN.uv_RGBA);
						fixed4 normalTex = tex2D(_normalmap, IN.uv_normalmap);
						fixed4 specTex = tex2D(_specTex_Gloss, IN.uv_specTex_Gloss);

						if (_useCustomColor > 0.5) {
							o.Albedo = (_maincolor.rgb * baseTex.rgb * (1.0 - normalTex.a))
									 + (lightTex.rgb * _lightRate)
									 + (_paintColor.rgb * normalTex.a);
						}
				 else {
				  o.Albedo = (_maincolor.rgb * baseTex.rgb) + (lightTex.rgb * _lightRate);
			  }

			  o.Normal = UnpackNormal(normalTex);
			  o.Specular = (specTex.rgb + _specColor.rgb) * _specMultiple;
			  o.Gloss = _shininess + specTex.a;
			  o.Alpha = 1.0; 
		  }

		  inline fixed4 LightingCustomLight(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
			  half3 h = normalize(lightDir + viewDir);
			  half diff = max(0, dot(s.Normal, lightDir));

			  half specPow = pow(max(0, dot(s.Normal, h)), s.Gloss * 128.0);
			  half3 spec = s.Specular * specPow;

			  fixed4 c;
			  c.rgb = (s.Albedo * _LightColor0.rgb * diff) + (spec * _LightColor0.rgb * atten * 2.0);
			  c.a = s.Alpha;
			  return c;
		  }
		  ENDCG
		}

			Fallback "StarWarfare/Middle/SNDM_new"
}