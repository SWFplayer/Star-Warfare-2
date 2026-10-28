Shader "MADFINGER/Transparent/Blinking GodRays" {
	Properties{
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

		SubShader{
		 LOD 100
		 Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

		 Pass {
		  Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
		  ZWrite Off
		  Cull Off
		  Fog { Color(0,0,0,0) }
		  Blend One One

		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #include "UnityCG.cginc"

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
		  float4 _Color;

		  struct appdata {
			  float4 vertex : POSITION;
			  float4 color : COLOR;
			  float3 normal : NORMAL;
			  float4 uv0 : TEXCOORD0;
		  };

		  struct v2f {
			  float4 pos : SV_POSITION;
			  float2 uv : TEXCOORD0;
			  fixed4 vertexColor : TEXCOORD1;
		  };

		  v2f vert(appdata v) {
			  v2f o;

			  float3 normal = normalize(v.normal);
			  float timeVal = _Time.y + (_BlinkingTimeOffsScale * v.color.z);

			  // View space distance calculation using UnityObjectToViewPos
			  float3 viewPos = UnityObjectToViewPos(v.vertex);
			  float dist = sqrt(dot(viewPos, viewPos));

			  // Distance fade calculations
			  float fadeNear = clamp(dist / _FadeOutDistNear, 0.0, 1.0);
			  float fadeFar = 1.0 - clamp((max(dist - _FadeOutDistFar, 0.0) * 0.2), 0.0, 1.0);

			  // Blinking wave calculations
			  float totalDuration = _TimeOnDuration + _TimeOffDuration;
			  float timeMod = timeVal / totalDuration;
			  float timeFrac = frac(abs(timeMod)) * totalDuration;
			  float waveTime = (timeMod >= 0.0) ? timeFrac : -timeFrac;

			  float pulseIn = clamp(waveTime / (_TimeOnDuration * 0.25), 0.0, 1.0);
			  float edge0 = _TimeOnDuration * 0.75;
			  float pulseOut = clamp((waveTime - edge0) / (_TimeOnDuration - edge0), 0.0, 1.0);

			  float pulseWave = (pulseIn * (pulseIn * (3.0 - (2.0 * pulseIn)))) * (1.0 - (pulseOut * (pulseOut * (3.0 - (2.0 * pulseOut)))));

			  // Noise calculations
			  float noiseAngle = timeVal * (6.28319 / _TimeOnDuration);
			  float noiseWave = (_NoiseAmount * (sin(noiseAngle) * ((0.5 * cos((noiseAngle * 0.6366) + 56.7272)) + 0.5))) + (1.0 - _NoiseAmount);

			  float sizeGrowFactor = min(max(dist - _SizeGrowStartDist, 0.0) / _SizeGrowEndDist, 1.0);
			  float finalBlink = (_NoiseAmount < 0.01) ? pulseWave : noiseWave;

			  float nearSq = fadeNear * fadeNear;

			  // Vertex position scaling along normals
			  float4 modifiedVertex = v.vertex;
			  modifiedVertex.xyz += (((sizeGrowFactor * sizeGrowFactor) * _MaxGrowSize) * v.color.w) * normal;

			  o.pos = UnityObjectToClipPos(modifiedVertex);
			  o.uv = v.uv0.xy;

			  // Final vertex color / intensity computation
			  o.vertexColor = (((nearSq * nearSq) * (fadeFar * fadeFar)) * _Color) * _Multiplier * (finalBlink + _Bias);

			  return o;
		  }

		  fixed4 frag(v2f i) : SV_Target {
			  fixed4 texColor = tex2D(_MainTex, i.uv);
			  return texColor * i.vertexColor;
		  }
		  ENDCG
		 }
	 }
}