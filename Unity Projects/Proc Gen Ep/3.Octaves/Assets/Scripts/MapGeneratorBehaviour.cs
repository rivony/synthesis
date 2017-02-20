using UnityEngine;

public class MapGeneratorBehaviour : MonoBehaviour {

	public int MapWidth;
	public int MapHeight;
	public float NoiseScale;

	public int OctavesNumber;
	[Range(0,1)]//turn the Persistance UI into a Slider
	public float Persistance;
	public float Lacunarity;

	public int Seed;
	public Vector2 Offset;

	public bool AutoUpdate;

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap (MapWidth, MapHeight, Seed, NoiseScale,
			OctavesNumber, Persistance, Lacunarity,
			Offset);

		MapDisplayBehaviour display = FindObjectOfType<MapDisplayBehaviour> ();
		display.DrawNoiseMap (noiseMap);
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
