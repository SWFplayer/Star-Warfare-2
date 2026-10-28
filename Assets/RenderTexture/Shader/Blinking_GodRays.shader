Shader "Advanced/Blinking_Atmosphere"
{
	Properties
	{
		_MainTex("Base texture", 2D) = "white" {}
		_FadeOutDistNear("Near fadeout dist", Float) = 10
		_FadeOutDistFar("Far fadeout dist", Float) = 10000
		_Multiplier("Color multiplier", Float) = 1
		_Bias("Bias", Float) = 0
		_TimeOnDuration("ON duration", Float) = 0.5
		_TimeOffDuration("OFF duration", Float) = 0.5
		_BlinkingTimeOffsScale("Blinking time offset scale (seconds)", Float) = 5
		_SizeGrowStartDist("Size grow start dist", Float) = 5
		_SizeGrowEndDist("Size grow end dist", Float) = 50
		_MaxGrowSize("Max grow size", Float) = 2.5
		_NoiseAmount("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
		_Color("Color", Color) = (1,1,1,1)
	}

		SubShader
		{
			LOD 100
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "true"
				"RenderType" = "Transparent"
			}

			Pass
			{
				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend One One 

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _FadeOutDistNear;
				float _FadeOutDistFar;
				float _Multiplier;
				float _Bias;
				float _TimeOnDuration;
				float _TimeOffDuration;
				float _BlinkingTimeOffsScale;
				float _SizeGrowStartDist;
				float _SizeGrowEndDist;
				float _MaxGrowSize;
				float _NoiseAmount;
				fixed4 _Color;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
					UNITY_FOG_COORDS(1)
				};

				v2f vert(appdata v)
				{
					v2f o;

					float t = _Time.y + (_BlinkingTimeOffsScale * v.color.z);

					float dist = distance(UnityObjectToViewPos(v.vertex), float3(0,0,0));

					float fadeNear = clamp(dist / _FadeOutDistNear, 0.0, 1.0);
					fadeNear = fadeNear * fadeNear; 
					fadeNear = fadeNear * fadeNear; 

					float fadeFar = 1.0 - clamp(max(dist - _FadeOutDistFar, 0.0) * 0.2, 0.0, 1.0);
					fadeFar = fadeFar * fadeFar; 

					float totalDuration = _TimeOnDuration + _TimeOffDuration;
					float timeInCycle = fmod(t, totalDuration);

					float t1 = clamp(timeInCycle / (_TimeOnDuration * 0.25), 0.0, 1.0);
					float smooth1 = t1 * t1 * (3.0 - (2.0 * t1));

					float edge0 = _TimeOnDuration * 0.75;
					float t2 = clamp((timeInCycle - edge0) / (_TimeOnDuration - edge0), 0.0, 1.0);
					float smooth2 = t2 * t2 * (3.0 - (2.0 * t2));

					float blinkPulse = smooth1 * (1.0 - smooth2);

					float noiseTime = t * (6.28319 / _TimeOnDuration); 
					float noiseVal = (_NoiseAmount * (sin(noiseTime) * ((0.5 * cos((noiseTime * 0.6366) + 56.7272)) + 0.5))) + (1.0 - _NoiseAmount);

					float finalBlink = (_NoiseAmount < 0.01) ? blinkPulse : noiseVal;

					float growAmount = min(max(dist - _SizeGrowStartDist, 0.0) / _SizeGrowEndDist, 1.0);
					growAmount = growAmount * growAmount * _MaxGrowSize * v.color.w;

					float3 grownPos = v.vertex.xyz + (growAmount * normalize(v.normal));
					o.pos = UnityObjectToClipPos(grownPos);

					o.color = (fadeNear * fadeFar * _Color * _Multiplier) * (finalBlink + _Bias);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed4 finalColor = tex * i.color;

					UNITY_APPLY_FOG(i.fogCoord, finalColor);
					return finalColor;
				}
				ENDCG
			}
		}
}