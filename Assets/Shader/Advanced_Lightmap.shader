Shader "Advanced/Lightmap" {
	Properties{
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecOffset("Specular Offset from Camera", Vector) = (1,10,2,0)
		_SpecRange("Specular Range", Float) = 20
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.01,1)) = 0.078125
		_ScrollingSpeed("Scrolling speed", Vector) = (0,0,0,0)
	}

		SubShader{
			LOD 100
			Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

			Pass {
				Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex   : POSITION;
					float3 normal   : NORMAL;
					float4 uv0      : TEXCOORD0;
					float4 uv1      : TEXCOORD1; // Lightmap UVs
				};

				struct v2f {
					float4 vertex   : SV_POSITION;
					float2 uv       : TEXCOORD0;
					float2 uvLM     : TEXCOORD1;
					fixed3 specular : TEXCOORD2;
					UNITY_FOG_COORDS(3)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				// sampler2D unity_Lightmap;
				// float4 unity_LightmapST;

				float3 _SpecOffset;
				float _SpecRange;
				float3 _SpecColor;
				float _Shininess;
				float4 _ScrollingSpeed;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					// 1. UV Scrolling (uses _Time.y which is standard seconds)
					o.uv = v.uv0.xy + frac(_ScrollingSpeed.xy * _Time.y);

					// 2. Lightmap UVs
					o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

					// 3. Fake Specular Math (Per-Vertex)
					// Get normal in view space
					float3 viewNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);

					// Calculate vertex position in view space
					float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;

					// Offset the specular highlight. Note the vec3(1,1,-1) in the GLSL
					float3 specPos = viewPos - (_SpecOffset * float3(1.0, 1.0, -1.0));

					// Half-vector between view direction (0,0,1) and light direction
					float3 halfVec = normalize(float3(0.0, 0.0, 1.0) + normalize(-specPos));

					// Blinn-Phong specular
					float ndoth = saturate(dot(normalize(viewNormal), halfVec));
					float specular = pow(ndoth, _Shininess * 128.0) * 2.0;

					// Distance falloff
					float dist = length(specPos);
					float falloff = 1.0 - saturate(dist / _SpecRange);

					o.specular = _SpecColor * specular * falloff;

					UNITY_TRANSFER_FOG(o, o.vertex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					// Sample base texture
					fixed4 tex = tex2D(_MainTex, i.uv);

				// Sample lightmap (baked lighting is doubled)
				fixed3 lm = 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM).rgb;

				// Combine: Base RGB + (Specular * Texture Alpha)
				fixed3 albedo = tex.rgb + (i.specular * tex.a);

				// Apply lighting
				fixed3 finalColor = albedo * lm;

				UNITY_APPLY_FOG(i.fogCoord, finalColor);

				return fixed4(finalColor, tex.a);
			}
			ENDCG
		}
		}
}