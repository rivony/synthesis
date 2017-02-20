using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdableData {
	
	public NormalizedMode NormalizeMode;

	public float NoiseScale;

	public int OctavesNumber;

	[Range(0,1)] //turn the Persistance UI into a Slider
	public float Persistance;
	public float Lacunarity;

	public int Seed;
	public Float2 Offset;

	protected override void OnValidate(){
		if (Lacunarity < 1) {
			Lacunarity = 1;
		}

		if (OctavesNumber < 0) {
			OctavesNumber = 0;
		}

		base.OnValidate ();
	}
}
