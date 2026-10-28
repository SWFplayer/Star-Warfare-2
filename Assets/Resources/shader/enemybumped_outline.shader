Shader "StarWarfare/Enemy/Bumped_Specular_Outline" {
	Properties{
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.03,1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_SpecMulti("Specular Multi", Float) = 1
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.002,0.1)) = 0.006
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 400

			Pass {
				Name "BASE"
				Tags { "RenderType" = "Opaque" }

				Stencil {
					Ref 2
					Pass Replace
					ZFail Replace
				}

			
			ColorMask 0
			ZWrite Off
		}

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

			CGPROGRAM
			#pragma surface surf CustomBumpedSpecularOutline fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
			fixed4 _SpecTint; 
			half _Shininess;
			half _SpecMulti;

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Albedo = mainTex.rgb;

				o.Specular = mainTex.a * _SpecMulti;
				o.Gloss = _Shininess;

				o.Alpha = 1.0;
			}

			inline fixed4 LightingCustomBumpedSpecularOutline(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half3 h = normalize(lightDir + viewDir);
				half diff = max(0, dot(s.Normal, lightDir));

				half nh = max(0, dot(s.Normal, h));
				half specPow = pow(nh, s.Gloss * 128.0);
				half spec = specPow * s.Specular;

				fixed4 c;

				c.rgb = ((s.Albedo * _LightColor0.rgb * diff) + (_LightColor0.rgb * _SpecTint.rgb * spec)) * 2.0;

				c.a = 1.0 + ((_LightColor0.a * _SpecTint.a) * spec);

				c.rgb *= atten;

				return c;
			}
			ENDCG
		}

			Fallback "Specular"
}