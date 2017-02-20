Shader "Custom/Terrain" {
	Properties {
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxColorCount = 8;

		//spelling of variables must respect the "Constants.cs" names
		int baseColorsCount;
		float3 baseColors[maxColorCount];
		float baseStartHeights[maxColorCount];

		float minHeight;
		float maxHeight;

		struct Input {
			float3 worldPos;
		};

		float InverseLerp(float minValue, float maxValue, float value){
			// Assuming that the value is in the range(minValue, maxValue)
			float result = (value - minValue) / (maxValue - minValue);
			return saturate(result); // Clamp the value between 0 and 1
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = InverseLerp(minHeight, maxHeight, IN.worldPos.y);

			for(int i = 0; i < baseColorsCount; i++){
				// we want the heightPercent be 0 if it is less to the BaseStartHeights
				float drawStrength = saturate(sign(heightPercent - baseStartHeights[i])); // clamp the value between -1 and 1
				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColors[i] * drawStrength);
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
