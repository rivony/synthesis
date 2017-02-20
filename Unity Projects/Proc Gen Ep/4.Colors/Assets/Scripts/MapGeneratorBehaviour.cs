using UnityEngine;

public class MapGeneratorBehaviour : MonoBehaviour {

	public int MapWidth;
	public int MapHeight;
	public float NoiseScale;

	public int OctavesNumber;
	[Range(0,1)] //turn the Persistance UI into a Slider
	public float Persistance;
	public float Lacunarity;

	public int Seed;
	public Float2 Offset;

	public TerrainType[] Regions;
	public DrawMode Mode;

	public bool AutoUpdate;

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap (MapWidth, MapHeight, Seed, NoiseScale,
			OctavesNumber, Persistance, Lacunarity,
			Offset);

		//Store the colors in respect of the Map height
		Color[] colorMap = new Color[MapWidth * MapHeight];

		//
		for (int y = 0; y < MapHeight; y++) {
			for (int x = 0; x < MapWidth; x++) {
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < Regions.Length; i++) {
					if (currentHeight <= Regions[i].Height) {
						colorMap [y * MapWidth + x] = Regions [i].TerrainColor;
						break;//no need to 
					}
				}
			}
		}

		MapDisplayBehaviour display = FindObjectOfType<MapDisplayBehaviour> ();

		if (Mode == DrawMode.NoiseMap) {
			display.DrawTexture(TextureGenerator.TextureFromHeightMap (noiseMap));
		}
		if (Mode == DrawMode.ColorMap) {
			display.DrawTexture (TextureGenerator.TextureFromColorMap (colorMap, MapWidth, MapHeight));
		}
	}

	//Automatically called whenever one of the script var has been changed by the inspector
	void OnValidate(){
		if (MapWidth < 1) {
			MapWidth = 1;
		}

		if (MapHeight < 1) {
			MapHeight = 1;
		}

		if (Lacunarity < 1) {
			Lacunarity = 1;
		}

		if (OctavesNumber < 0) {
			OctavesNumber = 0;
		}
	}
}