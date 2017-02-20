using UnityEngine;

public class MapGeneratorBehaviour : MonoBehaviour {

	//Constants
	const int MAP_CHUNCK_SIZE = 241; //241 - 1 = 240, a multiple of 2,4,6,8 for the LOD

	//public members
	[Range(0,6)]
	public int LevelOfDetail;// level of simplification: increment by 1

//	public int MapWidth;
//	public int MapHeight;

	public float NoiseScale;

	public int OctavesNumber;
	[Range(0,1)] //turn the Persistance UI into a Slider
	public float Persistance;
	public float Lacunarity;

	public int Seed;
	public Float2 Offset;

	public TerrainType[] Regions;
	public DrawMode Mode;

	public float MeshHeightMultiplier; //to elevate the heights
	public AnimationCurve MeshHeightCurve;//to flatten the water

	public bool AutoUpdate;

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap (MAP_CHUNCK_SIZE, MAP_CHUNCK_SIZE, Seed, NoiseScale,
			OctavesNumber, Persistance, Lacunarity,
			Offset);

		//Store the colors in respect of the Map height
		Color[] colorMap = new Color[MAP_CHUNCK_SIZE * MAP_CHUNCK_SIZE];

		//
		for (int y = 0; y < MAP_CHUNCK_SIZE; y++) {
			for (int x = 0; x < MAP_CHUNCK_SIZE; x++) {
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < Regions.Length; i++) {
					if (currentHeight <= Regions[i].Height) {
						colorMap [y * MAP_CHUNCK_SIZE + x] = Regions [i].TerrainColor;
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
			display.DrawTexture (TextureGenerator.TextureFromColorMap (colorMap, MAP_CHUNCK_SIZE,
				MAP_CHUNCK_SIZE));
		}
		if (Mode == DrawMode.Mesh) {
			display.DrawMesh (
				MeshGenerator.GenerateTerrainMesh(noiseMap, MeshHeightMultiplier, MeshHeightCurve,
					LevelOfDetail),
				TextureGenerator.TextureFromColorMap(colorMap, MAP_CHUNCK_SIZE, MAP_CHUNCK_SIZE));
		}
	}

	//Automatically called whenever one of the script var has been changed by the inspector
	void OnValidate(){
		if (Lacunarity < 1) {
			Lacunarity = 1;
		}

		if (OctavesNumber < 0) {
			OctavesNumber = 0;
		}
	}
}