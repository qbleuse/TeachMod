Shader "Hidden/Custom/OilPaint"
{
	/* based on the kuwahara filter, on this link: https://www.danielilett.com/2019-05-18-tut1-6-smo-painting/ */
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Radius;

	struct region
	{
		float3 mean;
		float variance;
	};

	region calcRegion(int2 lower, int2 upper, int samples, float2 uv)
	{
		region r;
		float3 sum = 0.0;
		float3 squareSum = 0.0;

		for (int x = lower.x; x <= upper.x; ++x)
		{
			for (int y = lower.y; y <= upper.y; ++y)
			{
				float2 offset = float2(x, y);
				float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);
				sum += tex.xyz;
				squareSum += tex.xyz * tex.xyz;
			}
		}

		r.mean = sum / samples;
		float3 variance = abs((squareSum / samples) - (r.mean * r.mean));
		r.variance = length(variance);

		return r;
	}

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		int upperBound = _Radius/2;
		int lowerBound = -_Radius/2;
		int samples = (upperBound + 1) * (upperBound + 1);

		// Calculate the four regional parameters as discussed.
		region regionA = calcRegion(int2(lowerBound, lowerBound), int2(0, 0), samples, i.texcoord);
		region regionB = calcRegion(int2(0, lowerBound), int2(upperBound, 0), samples, i.texcoord);
		region regionC = calcRegion(int2(lowerBound, 0), int2(0, upperBound), samples, i.texcoord);
		region regionD = calcRegion(int2(0, 0), int2(upperBound, upperBound), samples, i.texcoord);

		float3 col = regionA.mean;
		float minVar = regionA.variance;

		/*	Cascade through each region and compare variances - the end
			result will be the that the correct mean is picked for col.
		*/
		float testVal;

		testVal = step(regionB.variance, minVar);
		col = lerp(col, regionB.mean, testVal);
		minVar = lerp(minVar, regionB.variance, testVal);

		testVal = step(regionC.variance, minVar);
		col = lerp(col, regionC.mean, testVal);
		minVar = lerp(minVar, regionC.variance, testVal);

		testVal = step(regionD.variance, minVar);
		col = lerp(col, regionD.mean, testVal);

		return float4(col, 1.0);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}