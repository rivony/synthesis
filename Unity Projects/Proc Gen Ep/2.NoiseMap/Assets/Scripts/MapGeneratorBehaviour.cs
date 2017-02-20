using UnityEngine;

public class MapGeneratorBehaviour : MonoBehaviour {

	public int MapWidth;
	public int MapHeight;
	public float NoiseScale;

	public bool AutoUpdate;

	public void GenerateMap(){
		float[,] noiseMap = Noise.GenerateNoiseMap (MapWidth, MapHeight, NoiseScale);

		MapDisplayBehaviour display = FindObjectOfType<MapDisplayBehaviour> ();
		display.DrawNoiseMap (noiseMap);
	}
}
