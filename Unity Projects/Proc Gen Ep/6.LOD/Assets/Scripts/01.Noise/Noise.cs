using UnityEngine;
public static class Noise {

	public static float[,] GenerateNoiseMap (int mapWidth, int mapHeight, float scale) {
		float[,] noiseMap = new float[mapWidth, mapHeight];

		//divide by zero handler
		if (scale <= 0) {
			scale = 0.0001f;
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float sampleX = x/scale;
				float sampleY = y/scale;

				float perlinValue = Mathf.PerlinNoise (sampleX, sampleY);
				noiseMap [x, y] = perlinValue;
			}
		}

		return noiseMap;
	}

	public static float[,] GenerateNoiseMap (int mapWidth, int mapHeight, float scale,
		int octavesNumber, float persistance, float lacunarity) {

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float sampleX;
		float sampleY;
		float perlinValue;

		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;

		//divide by zero handler
		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				amplitude = 1;
				frequency = 1;
				noiseHeight = 0;

				for (int i = 0; i < octavesNumber; i++) {
					//take the frequency into account: 
					//the higher the frequency the further the sample points will be
					//the height value will change more rapidly
					sampleX = x / scale * frequency;
					sampleY = y / scale * frequency;
					
					perlinValue = Mathf.PerlinNoise (sampleX, sampleY); // give value between 0 and 1
					perlinValue = perlinValue * 2 - 1; //to get negative values so that the noiseHeight could decrease
					//Perlin value of each octaves
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance; // 0 < persistance < 1: decreases the octave
					frequency *= lacunarity; // the frequency increases each octave: lacunarity > 1
				}

				//to know the min/max noiseHeight
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				//to apply the noiseHeight to the noiseMap
				noiseMap[x, y] = noiseHeight;
			}
		}

		//"normalization" of the noiseMap so that the values get back to range(0,1), 
		//need to keep track the lowest and the highest values
		#warning (Rivo) need to optimise this
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
				//produces the interpolant value of noiseMap [x, y] within the range(minNoiseHeight, maxNoiseHeight)
				//if the values is exceding the max it is set to 1
				//if it is exceding the min it is set to 0
			}
		}

		return noiseMap;
	}

	public static float[,] GenerateNoiseMap (int mapWidth, int mapHeight, int seed, float scale,
		int octavesNumber, float persistance, float lacunarity,
		Float2 offset) {

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;

		float sampleX;
		float sampleY;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		float perlinValue;

		//divide by zero handler
		if (scale <= 0) {
			scale = 0.0001f;
		}

		System.Random prng = new System.Random (seed); //pseudo-random number generator (prng)
		Float2[] octaveOffsets = new Float2[octavesNumber];
		for (int i = 0; i < octavesNumber; i++) {
			//we don't want to give Mathf.PerlinNoise a coordinate that's too hight, 
			//otherwise it's just keep returning the same value over and over again 
			//range(-100000, 100000) works pretty well
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets[i] = new Float2(offsetX, offsetY);
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				amplitude = 1;
				frequency = 1;
				noiseHeight = 0;

				for (int i = 0; i < octavesNumber; i++) {
					//take the frequency into account: 
					//the higher the frequency the further the sample points will be
					//the height value will change more rapidly
					sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

					perlinValue = Mathf.PerlinNoise (sampleX, sampleY); // give value between 0 and 1
					perlinValue = perlinValue * 2 - 1; //to get negative values so that the noiseHeight could decrease
					//Perlin value of each octaves
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance; // 0 < persistance < 1: decreases the octave
					frequency *= lacunarity; // the frequency increases each octave: lacunarity > 1
				}

				//to know the min/max noiseHeight
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				//to apply the noiseHeight to the noiseMap
				noiseMap[x, y] = noiseHeight;
			}
		}

		//"normalization" of the noiseMap so that the values get back to range(0,1), 
		//need to keep track the lowest and the highest values
		#warning (Rivo) need to optimise this
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
				//produces the interpolant value of noiseMap [x, y] within the range(minNoiseHeight, maxNoiseHeight)
				//if the values is exceding the max it is set to 1
				//if it is exceding the min it is set to 0
			}
		}

		return noiseMap;
	}
}
