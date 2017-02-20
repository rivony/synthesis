Shader "Custom/Terrain" {
	Properties {
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4; //to avoid a division by zero if baseBlends[i] = 0, 10^-4

		//spelling of variables must respect the "Constants.cs" names
		int layerCount;
		float3 baseColors[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColorStrength[maxLayerCount];
		float baseTextureScales[maxLayerCount];

		float minHeight;
		float maxHeight;

		sampler2D testTexture;
		float testScale;

		UNITY_DECLARE_TEX2DARRAY(baseTextures);

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float InverseLerp(float minValue, float maxValue, float value){
			// Assuming that the value is in the range(minValue, maxValue)
			float result = (value - minValue) / (maxValue - minValue);
			return saturate(result); // Clamp the value between 0 and 1
		}

		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex){

			float3 scaleWorldPos = worldPos / scale;


			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.y, scaleWorldPos.z, textureIndex)) * blendAxes.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.z, textureIndex)) * blendAxes.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.y, textureIndex)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = InverseLerp(minHeight, maxHeight, IN.worldPos.y);

			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= (blendAxes.x + blendAxes.y + blendAxes.z); // to make sure that the sum of the blendAxes don't exceed 1

			for(int i = 0; i < layerCount; i++){
				// we want the heightPercent be 0 if it is less to the BaseStartHeights
				float drawStrength = InverseLerp(- baseBlends[i]*0.5 - epsilon, baseBlends[i]*0.5,
				  heightPercent - baseStartHeights[i]); // clamp the value between -1 and 1

				float3 baseColor = baseColors[i] * baseColorStrength[i];
				float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]);

				o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
			}


			// o.Albedo = xProjection + yProjection + zProjection;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
