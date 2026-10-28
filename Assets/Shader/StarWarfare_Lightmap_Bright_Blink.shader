Shader "StarWarfare/Lightmap_Bright_Blink" {
	Properties{
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecOffset("Specular Offset from Camera", Vector) = (1,10,2,0)
		_SpecRange("Specular Range", Float) = 20
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.01,1)) = 0.078125
		_ScrollingSpeed("Scrolling speed", Vector) = (0,0,0,0)
		_ColorOffset("Color Offset", Float) = 1
		_tex2("Texture2", 2D) = "" {}
		_FadeOutDistNear("Near fadeout dist", Float) = 0
		_FadeOutDistFar("Far fadeout dist", Float) = 1000
		_Multiplier("Color multiplier", Float) = 1
		_Bias("Bias", Float) = 0.1
		_TimeOnDuration("ON duration", Float) = 0.5
		_TimeOffDuration("OFF duration", Float) = 0.5
		_BlinkingTimeOffsScale("Blinking time offset scale (seconds)", Float) = 5
		_SizeGrowStartDist("Size grow start dist", Float) = 5
		_SizeGrowEndDist("Size grow end dist", Float) = 50
		_MaxGrowSize("Max grow size", Float) = 2.5
		_NoiseAmount("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
		_Color("Color", Color) = (1,1,1,1)
	}

		SubShader{
			LOD 100
			Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

			Pass {
				Tags { "LIGHTMODE" = "ForwardBase" "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog 
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;   
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 lmUV : TEXCOORD1;
					float3 specular : TEXCOORD2;
					float4 color : COLOR;
					UNITY_FOG_COORDS(3)
				};

				sampler2D _MainTex;
				sampler2D _tex2;
				float4 _SpecOffset;
				float _SpecRange;
				float4 _SpecColor;
				float _Shininess;
				float4 _ScrollingSpeed;
				float _ColorOffset;
				float _FadeOutDistNear;
				float _FadeOutDistFar;
				float _Multiplier;
				float _Bias;
				float _TimeOnDuration;
				float _TimeOffDuration;
				float _BlinkingTimeOffsScale;
				float _NoiseAmount;
				float4 _Color;

				v2f vert(appdata v) {
					v2f o;

					float4 mvPos = mul(UNITY_MATRIX_MV, v.vertex);
					o.pos = mul(UNITY_MATRIX_P, mvPos);

					o.uv = v.uv + frac(_ScrollingSpeed.xy * _Time.y);
					o.lmUV = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;

					float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));

					float3 specOffsetView = _SpecOffset * float3(1, 1, -1);
					float3 specVec = mvPos.xyz - specOffsetView;

					float3 viewDir = normalize(-specVec);
					float3 halfVec = normalize(float3(0, 0, 1) + viewDir);

					float NdotH = clamp(dot(viewNormal, halfVec), 0, 1);
					float specPower = pow(NdotH, _Shininess * 128.0);

					float dist = sqrt(dot(specVec, specVec));
					float distFade = 1.0 - clamp(dist / _SpecRange, 0, 1);

					o.specular = (_SpecColor.rgb * specPower * 2.0) * distFade;

					float viewDist = length(mvPos.xyz);
					float nearFade = clamp(viewDist / _FadeOutDistNear, 0, 1);
					float farFade = 1.0 - clamp(max(viewDist - _FadeOutDistFar, 0) * 0.2, 0, 1);

					float fade = (nearFade * nearFade) * (farFade * farFade);

					float time = _Time.y + (_BlinkingTimeOffsScale * v.color.z);

					float totalDuration = _TimeOnDuration + _TimeOffDuration;
					float t = frac(abs(time / totalDuration)) * totalDuration;
					if (time < 0) t = -t;

					float rise = clamp(t / (_TimeOnDuration * 0.25), 0, 1);
					float fall = clamp((t - _TimeOnDuration * 0.75) / (_TimeOnDuration * 0.25), 0, 1);

					float riseSmooth = rise * rise * (3.0 - 2.0 * rise);
					float fallSmooth = fall * fall * (3.0 - 2.0 * fall);
					float pulse = riseSmooth * (1.0 - fallSmooth);

					float noiseFreq = time * (6.28319 / _TimeOnDuration);
					float noise = _NoiseAmount * (sin(noiseFreq) * (0.5 * cos(noiseFreq * 0.6366 + 56.7272) + 0.5)) + (1.0 - _NoiseAmount);

					float blink = (_NoiseAmount < 0.01) ? pulse : noise;

					o.color = ((fade * _Color) * _Multiplier) * (blink + _Bias);

					UNITY_TRANSFER_FOG(o, o.pos);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 mainTex = tex2D(_MainTex, i.uv);

				fixed4 tex2 = tex2D(_tex2, i.uv);

				fixed3 color = mainTex.rgb + (i.specular * mainTex.a);

				fixed3 lm = 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmUV).rgb;
				color *= lm * _ColorOffset;

				color += (i.color.rgb * tex2.rgb);

				fixed4 result = fixed4(color, mainTex.a);

				UNITY_APPLY_FOG(i.fogCoord, result);

				return result;
			}
			ENDCG
		}
		}
}