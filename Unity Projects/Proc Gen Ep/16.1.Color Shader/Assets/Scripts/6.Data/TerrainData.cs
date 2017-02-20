using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdableData {
	public float UniformScale = 2.5f;

	public bool UseFlatShading;
	public bool UseFalloff;

	public float MeshHeightMultiplier; //to elevate the heights
	public AnimationCurve MeshHeightCurve; //to flatten the water

	public float minHeight {
		get{ 
			return UniformScale * MeshHeightMultiplier * MeshHeightCurve.Evaluate (0);
		}
	}

	public float maxHeight {
		get{ 
			return UniformScale * MeshHeightMultiplier * MeshHeightCurve.Evaluate (1);
		}
	}
}
